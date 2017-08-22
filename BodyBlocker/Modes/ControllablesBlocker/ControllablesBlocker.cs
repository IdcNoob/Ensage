namespace BodyBlocker.Modes.ControllablesBlocker
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;

    internal class ControllablesBlocker : IBodyBlockMode
    {
        private readonly Unit hero;

        private Hero blockTarget;

        private ControllablesBlockerSettings settings;

        private TaskHandler taskHandler;

        [ImportingConstructor]
        public ControllablesBlocker(IServiceContext context)
        {
            hero = context.Owner;
        }

        public void Activate()
        {
            settings = new ControllablesBlockerSettings();
            settings.Key.Item.ValueChanged += KeyPressed;

            taskHandler = UpdateManager.Run(OnUpdate, true, false);
        }

        public void Dispose()
        {
            settings.Key.Item.ValueChanged -= KeyPressed;
            taskHandler.Cancel();
        }

        private void KeyPressed(object sender, OnValueChangeEventArgs onValueChangeEventArgs)
        {
            if (!onValueChangeEventArgs.GetNewValue<KeyBind>())
            {
                return;
            }

            if (taskHandler.IsRunning)
            {
                taskHandler.Cancel();
                return;
            }

            blockTarget = EntityManager<Hero>.Entities.OrderBy(x => x.Distance2D(Game.MousePosition))
                .FirstOrDefault(
                    x => x.IsValid && x.IsVisible && !x.IsIllusion && x.IsAlive
                         && x.Distance2D(Game.MousePosition) < 1000 && x.IsEnemy(hero)
                         && !x.UnitState.HasFlag(UnitState.NoCollision));

            if (blockTarget != null)
            {
                taskHandler.RunAsync();
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused)
            {
                return;
            }

            if (!blockTarget.IsAlive || !blockTarget.IsVisible)
            {
                taskHandler.Cancel();
                return;
            }

            var controllables = EntityManager<Unit>.Entities.Where(
                    x => x.IsValid && !x.Equals(hero) && x.IsAlive && x.IsAlly(hero) && x.IsControllable
                         && x.AttackCapability != AttackCapability.None && x.MoveCapability == MoveCapability.Ground
                         && !x.UnitState.HasFlag(UnitState.NoCollision) && x.Distance2D(blockTarget) < 1000)
                .ToList();

            var count = Math.Min(controllables.Count, settings.ControllablesCount);
            if (count <= 0)
            {
                taskHandler.Cancel();
                return;
            }

            for (var i = 0; i < count; i++)
            {
                var targetPosition = blockTarget.NetworkPosition;
                var definitelynotrandomnumbers = settings.SpreadUnits
                                                     ? (float)Math.Ceiling(i / 2f)
                                                       * (i % 2 == 0 ? -1f + count * 0.06f : 1f - count * 0.06f)
                                                     : 0;
                var blockPosition = targetPosition + blockTarget.Vector3FromPolarAngle(definitelynotrandomnumbers)
                                    * settings.BlockSensitivity;

                var controllable = controllables.OrderBy(x => x.Distance2D(blockPosition)).First();
                controllables.Remove(controllable);

                var angle = blockTarget.FindRotationAngle(controllable.Position);

                if (angle > 1.1 + i * 0.5)
                {
                    var delta = angle * 0.6f;
                    var side1 = targetPosition + blockTarget.Vector3FromPolarAngle(delta)
                                * Math.Max(settings.BlockSensitivity, 150);
                    var side2 = targetPosition + blockTarget.Vector3FromPolarAngle(-delta)
                                * Math.Max(settings.BlockSensitivity, 150);

                    controllable.Move(
                        side1.Distance(controllable.Position) < side2.Distance(controllable.Position) ? side1 : side2);
                }
                else
                {
                    if (blockTarget.IsMoving && angle < 0.3 && controllable.IsMoving)
                    {
                        controllable.Stop();
                    }
                    else
                    {
                        controllable.Move(blockPosition);
                    }
                }

                //delay between each issued command
                await Task.Delay(50, token);
            }
        }
    }
}
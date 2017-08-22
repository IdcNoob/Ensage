namespace BodyBlocker.Modes.HeroBlocker
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;

    internal class HeroBlocker : IBodyBlockMode
    {
        private readonly Unit hero;

        private HeroBlockerSettings settings;

        private IUpdateHandler updateHandler;

        [ImportingConstructor]
        public HeroBlocker(IServiceContext context)
        {
            hero = context.Owner;
        }

        public void Activate()
        {
            settings = new HeroBlockerSettings();
            settings.Key.Item.ValueChanged += KeyPressed;
            updateHandler = UpdateManager.Subscribe(OnUpdate, 50, false);
        }

        public void Dispose()
        {
            settings.Key.Item.ValueChanged -= KeyPressed;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void KeyPressed(object sender, OnValueChangeEventArgs onValueChangeEventArgs)
        {
            updateHandler.IsEnabled = onValueChangeEventArgs.GetNewValue<KeyBind>();

            if (!settings.CenterCamera)
            {
                return;
            }

            const string Command = "dota_camera_center_on_hero";
            Game.ExecuteCommand((updateHandler.IsEnabled ? "+" : "-") + Command);
        }

        private void OnUpdate()
        {
            if (!hero.IsAlive || Game.IsPaused)
            {
                return;
            }

            var blockTarget = EntityManager<Hero>.Entities.OrderBy(x => x.Distance2D(hero))
                .FirstOrDefault(
                    x => x.IsValid && x.IsVisible && x.IsAlive && !x.IsIllusion && x.Distance2D(hero) < 1000
                         && x.IsEnemy(hero) && !x.UnitState.HasFlag(UnitState.NoCollision));

            if (blockTarget == null)
            {
                return;
            }

            var angle = blockTarget.FindRotationAngle(hero.Position);

            if (angle > 1.3)
            {
                var delta = angle * 0.6f;
                var position = blockTarget.NetworkPosition;
                var side1 = position + blockTarget.Vector3FromPolarAngle(delta)
                            * Math.Max(settings.BlockSensitivity, 150);
                var side2 = position + blockTarget.Vector3FromPolarAngle(-delta)
                            * Math.Max(settings.BlockSensitivity, 150);

                hero.Move(side1.Distance(hero.Position) < side2.Distance(hero.Position) ? side1 : side2);
            }
            else
            {
                if (blockTarget.IsMoving && angle < 0.3 && hero.IsMoving)
                {
                    hero.Stop();
                    return;
                }

                hero.Move(blockTarget.InFront(settings.BlockSensitivity));
            }
        }
    }
}
namespace CreepsBlocker
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
    using Ensage.SDK.Service.Metadata;

    using SharpDX;

    [ExportPlugin("Creeps Blocker", StartupMode.Auto, "IdcNoob")]
    internal class CreepsBlocker : Plugin
    {
        private readonly Unit hero;

        private Config config;

        private IUpdateHandler updateHandler;

        [ImportingConstructor]
        public CreepsBlocker([Import] IServiceContext context)
        {
            hero = context.Owner;
        }

        protected override void OnActivate()
        {
            config = new Config();
            config.Key.Item.ValueChanged += KeyPressed;
            updateHandler = UpdateManager.Subscribe(OnUpdate, 50, false);

            Game.PrintMessage("<font color='#ff7700'>Creeps Blocker is obsolete, use Body Blocker instead</font>");
        }

        protected override void OnDeactivate()
        {
            config.Key.Item.ValueChanged -= KeyPressed;
            UpdateManager.Unsubscribe(OnUpdate);
            config.Dispose();
        }

        private void KeyPressed(object sender, OnValueChangeEventArgs onValueChangeEventArgs)
        {
            updateHandler.IsEnabled = onValueChangeEventArgs.GetNewValue<KeyBind>();

            if (!config.CenterCamera)
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

            var creeps = EntityManager<Creep>.Entities.Where(
                    x => x.IsValid && x.IsSpawned && x.IsAlive && x.Team == hero.Team && x.Distance2D(hero) < 500)
                .ToList();

            if (!creeps.Any())
            {
                return;
            }

            var creepsMoveDirection = creeps.Aggregate(new Vector3(), (sum, creep) => sum + creep.InFront(1000))
                                      / creeps.Count;

            var tower = EntityManager<Tower>.Entities.FirstOrDefault(
                x => x.IsValid && x.IsAlive && x.Distance2D(hero) < 500 && x.Name == "npc_dota_badguys_tower2_mid");

            if (tower?.Distance2D(hero) < 120)
            {
                // dont block near retarded dire mid t2 tower
                hero.Move(creepsMoveDirection);
                return;
            }

            foreach (var creep in creeps.OrderByDescending(x => x.IsMoving)
                .ThenBy(x => x.Distance2D(creepsMoveDirection)))
            {
                if (!config.BlockRangedCreep && creep.IsRanged)
                {
                    continue;
                }

                var creepDistance = creep.Distance2D(creepsMoveDirection) + 50;
                var heroDistance = hero.Distance2D(creepsMoveDirection);
                var creepAngle = creep.FindRotationAngle(hero.Position);

                if (creepDistance < heroDistance && creepAngle > 2 || creepAngle > 2.5)
                {
                    continue;
                }

                var moveDistance = (float)config.BlockSensitivity / hero.MovementSpeed * 100;
                var movePosition = creep.InFront(Math.Max(moveDistance, moveDistance * creepAngle));

                if (movePosition.Distance(creepsMoveDirection) > heroDistance)
                {
                    continue;
                }

                if (creepAngle < 0.3)
                {
                    if (hero.MovementSpeed - creep.MovementSpeed > 50
                        && creeps.Select(x => x.FindRotationAngle(hero.Position)).Average() < 0.4)
                    {
                        hero.Stop();
                        return;
                    }

                    continue;
                }

                hero.Move(movePosition);
                return;
            }

            hero.Stop();
        }
    }
}
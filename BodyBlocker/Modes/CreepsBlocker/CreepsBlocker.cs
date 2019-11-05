namespace BodyBlocker.Modes.CreepsBlocker
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;

    using SharpDX;

    internal class CreepsBlocker : IBodyBlockMode
    {
        private readonly Unit hero;

        private readonly Map map;

        private CreepsBlockerSettings settings;

        private IUpdateHandler updateHandler;

        [ImportingConstructor]
        public CreepsBlocker(IServiceContext context, Map map)
        {
            hero = context.Owner;
            this.map = map;
        }

        public void Activate()
        {
            settings = new CreepsBlockerSettings();
            settings.Key.Item.ValueChanged += KeyPressed;
            updateHandler = UpdateManager.Subscribe(OnUpdate, 50, false);
        }

        public void Dispose()
        {
            settings.Key.Item.ValueChanged -= KeyPressed;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private Vector3 GetMovePosition(IReadOnlyCollection<Creep> creeps)
        {
            if (!creeps.Any())
            {
                return new Vector3();
            }

            var route = map.GetCreepRoute(creeps.First());
            if (!route.Any())
            {
                return new Vector3();
            }

            //hack wrong route points
            if (creeps.First().Team == Team.Dire && map.GetLane(creeps.First()) == MapArea.Middle)
            {
                route = route.Skip(1).ToList();
            }

            var creepsMoveDirection =
                creeps.Aggregate(new Vector3(), (sum, creep) => sum + creep.InFront(300)) / creeps.Count;

            var point = route.IndexOf(route.OrderBy(x => x.Distance(creepsMoveDirection)).First());

            return route[Math.Min(point + 1, route.Count - 1)];
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

            var creeps = EntityManager<Creep>.Entities.Where(
                    x => x.IsValid && x.IsSpawned && x.IsAlive && x.Team == hero.Team && x.Distance2D(hero) < 600)
                .ToList();

            var creepsMovePosition = GetMovePosition(creeps);
            if (creepsMovePosition.IsZero)
            {
                return;
            }

            var tower = EntityManager<Tower>.Entities.FirstOrDefault(
                x => x.IsValid && x.IsAlive && x.Distance2D(hero) < 200 && x.Name == "npc_dota_badguys_tower2_mid");

            if (tower != null)
            {
                // dont block near retarded dire mid t2 tower
                hero.Move(hero.Position.Extend(creepsMovePosition, 200));
                return;
            }

            foreach (var creep in creeps.OrderBy(x => x.Distance2D(creepsMovePosition)))
            {
                if (!settings.BlockRangedCreep && creep.IsRanged)
                {
                    continue;
                }

                if (!creep.IsMoving && creep.Distance2D(hero) > 50)
                {
                    continue;
                }

                var creepDistance = creep.Distance2D(creepsMovePosition) + 50;
                var heroDistance = hero.Distance2D(creepsMovePosition);
                var creepAngle = creep.FindRotationAngle(hero.NetworkPosition);

                if (creepDistance < heroDistance && creepAngle > 2 || creepAngle > 2.5)
                {
                    continue;
                }

                var moveDistance = (float)settings.BlockSensitivity / hero.MovementSpeed * 100;
                if (hero.MovementSpeed - creep.MovementSpeed > 50)
                {
                    moveDistance -= (hero.MovementSpeed - creep.MovementSpeed) / 2;
                }
                var movePosition = creep.InFront(Math.Max(moveDistance, moveDistance * creepAngle));

                if (movePosition.Distance(creepsMovePosition) - 50 > heroDistance)
                {
                    continue;
                }

                if (creepAngle < 0.2 && hero.IsMoving)
                {
                    continue;
                }

                hero.Move(movePosition);
                return;
            }

            if (hero.IsMoving)
            {
                hero.Stop();
            }
            else if (hero.FindRotationAngle(creepsMovePosition) > 1.5)
            {
                hero.Move(hero.Position.Extend(creepsMovePosition, 10));
            }
        }
    }
}
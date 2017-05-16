namespace CreepsBlocker
{
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using SharpDX;

    [ExportPlugin("Creeps blocker", StartupMode.Auto, "IdcNoob")]
    internal class CreepsBlocker : Plugin
    {
        private readonly Hero hero;

        private Config config;

        [ImportingConstructor]
        public CreepsBlocker([Import] IServiceContext context)
        {
            hero = context.Owner;
        }

        protected override void OnActivate()
        {
            config = new Config();
            UpdateManager.Subscribe(OnUpdate, 100);
        }

        protected override void OnDeactivate()
        {
            UpdateManager.Unsubscribe(OnUpdate);
            config.Dispose();
        }

        private void OnUpdate()
        {
            if (!config.Enabled || !hero.IsAlive || Game.IsPaused)
            {
                return;
            }

            var creeps = EntityManager<Creep>.Entities
                .Where(x => x.IsValid && x.IsSpawned && x.IsAlive && x.Team == hero.Team && x.Distance2D(hero) < 700)
                .ToList();

            if (!creeps.Any())
            {
                return;
            }

            var creepsMoveDirection = creeps.Aggregate(new Vector3(), (sum, creep) => sum + creep.InFront(1000))
                                      / creeps.Count;

            var blockCreep = creeps.OrderBy(x => x.Distance2D(creepsMoveDirection))
                .FirstOrDefault(
                    x => x.Distance2D(creepsMoveDirection) > hero.Distance2D(creepsMoveDirection)
                         && (!x.IsRanged || config.BlockRangedCreep));

            if (blockCreep == null || hero.Distance2D(blockCreep) > 50
                && blockCreep.FindRotationAngle(hero.Position) < 0.15)
            {
                return;
            }

            var movePosition = blockCreep.InFront(450f / hero.MovementSpeed * 100);
            if (hero.FindRotationAngle(movePosition) > 2.25)
            {
                return;
            }

            hero.Move(movePosition);
        }
    }
}
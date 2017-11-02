namespace VisionControl.Heroes
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using SharpDX;

    using Units.Interfaces;

    internal class EnemyHero
    {
        private const AbilityId DispenserId = AbilityId.item_ward_dispenser;

        private const AbilityId ObserverId = AbilityId.item_ward_observer;

        private const AbilityId SentryId = AbilityId.item_ward_sentry;

        private readonly Hero hero;

        public EnemyHero(Hero enemy)
        {
            hero = enemy;
            Handle = enemy.Handle;
            ObserversCount = CountWards(ObserverId);
            SentryCount = CountWards(SentryId);
        }

        public uint Handle { get; }

        public bool IsAlive => hero.IsAlive;

        public bool IsValid => hero.IsValid;

        public bool IsVisible => IsValid && hero.IsVisible;

        public uint ObserversCount { get; set; }

        public Vector3 Position => hero.Position;

        public uint SentryCount { get; set; }

        public double Angle(IWard ward)
        {
            return hero.FindRelativeAngle(ward.Position);
        }

        public uint CountWards(AbilityId id)
        {
            var items = hero.Inventory.Items.Concat(hero.Inventory.Backpack).Where(x => x.IsValid).ToList();
            return (uint)(items.Where(x => x.Id == id).Sum(x => x.CurrentCharges) + items.Where(x => x.Id == DispenserId)
                              .Sum(x => id == AbilityId.item_ward_observer ? x.CurrentCharges : x.SecondaryCharges));
        }

        public float Distance(EnemyHero enemy)
        {
            return hero.Distance2D(enemy.Position);
        }

        public bool DroppedWard(AbilityId id)
        {
            return EntityManager<PhysicalItem>.Entities.Any(x => (x.Item.Id == id || x.Item.Id == DispenserId) && x.Distance2D(hero) < 300);
        }

        public uint GetWardsCount(AbilityId id)
        {
            return id == ObserverId ? ObserversCount : SentryCount;
        }

        public void SetWardsCount(AbilityId id, uint count)
        {
            if (id == ObserverId)
            {
                ObserversCount = count;
            }
            else
            {
                SentryCount = count;
            }
        }

        public Vector3 WardPosition()
        {
            return hero.InFront(350);
        }
    }
}
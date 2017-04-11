namespace VisionControl.Heroes
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    using Units.Wards;

    internal class EnemyHero
    {
        private const string DispenserName = "item_ward_dispenser";

        private const string ObserverName = "item_ward_observer";

        private const string SentryName = "item_ward_sentry";

        private readonly Hero hero;

        public EnemyHero(Hero enemy)
        {
            hero = enemy;
            Handle = enemy.Handle;
            ObserversCount = CountObservers();
            SentryCount = CountSentries();
        }

        public uint Handle { get; }

        public bool IsAlive => hero.IsAlive;

        public bool IsValid => hero.IsValid;

        public bool IsVisible => IsValid && hero.IsVisible;

        public uint ObserversCount { get; set; }

        public Vector3 Position => hero.Position;

        public uint SentryCount { get; set; }

        public double Angle(Ward ward)
        {
            return hero.FindRelativeAngle(ward.Position);
        }

        public uint CountObservers()
        {
            return (hero.FindItem(ObserverName)?.CurrentCharges ?? 0)
                   + (hero.FindItem(DispenserName)?.CurrentCharges ?? 0)
                   + (hero.Inventory.Backpack.FirstOrDefault(x => x.IsValid && x.Name == ObserverName)?.CurrentCharges
                      ?? 0) + (hero.Inventory.Backpack.FirstOrDefault(x => x.IsValid && x.Name == DispenserName)
                                   ?.CurrentCharges ?? 0);
        }

        public uint CountSentries()
        {
            return (hero.FindItem(SentryName)?.CurrentCharges ?? 0)
                   + (hero.FindItem(DispenserName)?.SecondaryCharges ?? 0)
                   + (hero.Inventory.Backpack.FirstOrDefault(x => x.IsValid && x.Name == SentryName)?.CurrentCharges
                      ?? 0) + (hero.Inventory.Backpack.FirstOrDefault(x => x.IsValid && x.Name == DispenserName)
                                   ?.SecondaryCharges ?? 0);
        }

        public uint CountWards(ClassId id)
        {
            return id == ClassId.CDOTA_Item_ObserverWard ? CountObservers() : CountSentries();
        }

        public float Distance(EnemyHero enemy)
        {
            return hero.Distance2D(enemy.Position);
        }

        public bool DroppedWard(ClassId id)
        {
            return ObjectManager.GetEntities<PhysicalItem>().Any(x => x.Item.ClassId == id && x.Distance2D(hero) < 100);
        }

        public uint GetWardsCount(ClassId id)
        {
            return id == ClassId.CDOTA_Item_ObserverWard ? ObserversCount : SentryCount;
        }

        public void SetWardsCount(ClassId id, uint count)
        {
            if (id == ClassId.CDOTA_Item_ObserverWard)
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
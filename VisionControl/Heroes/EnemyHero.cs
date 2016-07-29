namespace VisionControl.Heroes
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class EnemyHero
    {
        #region Constants

        private const string DispenserName = "item_ward_dispenser";

        private const string ObserverName = "item_ward_observer";

        private const string SentryName = "item_ward_sentry";

        #endregion

        #region Fields

        private readonly Hero hero;

        #endregion

        #region Constructors and Destructors

        public EnemyHero(Hero enemy)
        {
            hero = enemy;
            Handle = enemy.Handle;
            ObserversCount = CountObservers();
            SentryCount = CountSentries();
        }

        #endregion

        #region Public Properties

        public uint Handle { get; }

        public bool IsAlive => hero.IsAlive;

        public bool IsVisible => hero.IsValid && hero.IsVisible;

        public uint ObserversCount { get; set; }

        public Vector3 Position => hero.Position;

        public uint SentryCount { get; set; }

        #endregion

        #region Public Methods and Operators

        public uint CountObservers()
        {
            return (hero.FindItem(ObserverName)?.CurrentCharges ?? 0)
                   + (hero.FindItem(DispenserName)?.CurrentCharges ?? 0);
        }

        public uint CountSentries()
        {
            return (hero.FindItem(SentryName)?.CurrentCharges ?? 0)
                   + (hero.FindItem(DispenserName)?.SecondaryCharges ?? 0);
        }

        public float Distance(EnemyHero enemy)
        {
            return hero.Distance2D(enemy.Position);
        }

        public bool DroppedWard(ClassID id)
        {
            return ObjectManager.GetEntities<PhysicalItem>().Any(x => x.Item.ClassID == id && x.Distance2D(hero) < 100);
        }

        public Vector3 WardPosition()
        {
            return hero.InFront(350);
        }

        #endregion
    }
}
namespace ItemManager.Core.Modules.Snatcher.Controllables
{
    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    internal abstract class Controllable
    {
        protected Controllable(Unit unit)
        {
            Unit = unit;
            Handle = unit.Handle;
            Sleeper = new Sleeper();
        }

        public uint Handle { get; }

        protected Sleeper Sleeper { get; }

        protected Unit Unit { get; }

        public abstract bool CanPick(PhysicalItem physicalItem, int costThreshold);

        public abstract bool CanPick(Rune rune);

        public bool IsValid()
        {
            return Unit != null && Unit.IsValid && Unit.IsAlive;
        }

        public void Pick(PhysicalItem item)
        {
            if (Unit.PickUpItem(item))
            {
                Sleeper.Sleep(500);
            }
        }

        public void Pick(Rune rune)
        {
            if (Unit.PickUpRune(rune))
            {
                Sleeper.Sleep(500);
            }
        }
    }
}
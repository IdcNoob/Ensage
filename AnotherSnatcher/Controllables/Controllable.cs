namespace AnotherSnatcher.Controllables
{
    using Ensage;

    internal abstract class Controllable
    {
        #region Constructors and Destructors

        protected Controllable(Unit unit)
        {
            Unit = unit;
            Handle = unit.Handle;
        }

        #endregion

        #region Public Properties

        public uint Handle { get; }

        #endregion

        #region Properties

        protected Unit Unit { get; }

        #endregion

        #region Public Methods and Operators

        public abstract bool CanPick(PhysicalItem physicalItem);

        public abstract bool CanPick(Rune rune);

        public bool IsValid()
        {
            return Unit != null && Unit.IsValid && Unit.IsAlive;
        }

        public void Pick(PhysicalItem item)
        {
            Unit.PickUpItem(item);
        }

        public void Pick(Rune rune)
        {
            Unit.PickUpRune(rune);
        }

        #endregion
    }
}
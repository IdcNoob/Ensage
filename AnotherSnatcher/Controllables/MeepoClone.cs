namespace AnotherSnatcher.Controllables
{
    using Ensage;
    using Ensage.Common.Extensions;

    internal class MeepoClone : Controllable
    {
        #region Constructors and Destructors

        public MeepoClone(Unit unit)
            : base(unit)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanPick(PhysicalItem physicalItem)
        {
            return false;
        }

        public override bool CanPick(Rune rune)
        {
            return Unit.Distance2D(rune) < 400;
        }

        #endregion
    }
}
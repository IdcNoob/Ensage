namespace AnotherSnatcher.Controllables
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class SpiritBear : Controllable
    {
        #region Constructors and Destructors

        public SpiritBear(Unit unit)
            : base(unit)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanPick(PhysicalItem physicalItem)
        {
            if (Unit.Distance2D(physicalItem) > 400)
            {
                return false;
            }

            switch (physicalItem.Item.ID)
            {
                case 30:
                case 133:
                    return Unit.Inventory.FreeSlots.Any();
                case 117:
                    return false;
                case 33:
                    return Unit.Inventory.FreeSlots.Any() || Unit.Inventory.FreeBackpackSlots.Any();
            }

            return false;
        }

        public override bool CanPick(Rune rune)
        {
            return Unit.Distance2D(rune) < 400;
        }

        #endregion
    }
}
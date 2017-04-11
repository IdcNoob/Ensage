namespace ItemManager.Core.Modules.Snatcher.Controllables
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class MyHero : Controllable
    {
        public MyHero(Unit unit)
            : base(unit)
        {
        }

        public override bool CanPick(PhysicalItem physicalItem)
        {
            if (Unit.Distance2D(physicalItem) > 400)
            {
                return false;
            }

            switch (physicalItem.Item.Id)
            {
                case AbilityId.item_gem:
                case AbilityId.item_rapier:
                case AbilityId.item_aegis:
                {
                    return Unit.Inventory.FreeInventorySlots.Any();
                }
                case AbilityId.item_cheese:
                {
                    return Unit.Inventory.FreeInventorySlots.Any() || Unit.Inventory.FreeBackpackSlots.Any();
                }
            }

            return false;
        }

        public override bool CanPick(Rune rune)
        {
            return Unit.Distance2D(rune) < 400;
        }
    }
}
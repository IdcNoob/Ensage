namespace ItemManager.Core.Modules.Snatcher.Controllables
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    internal class SpiritBear : Controllable
    {
        public SpiritBear(Unit unit)
            : base(unit)
        {
        }

        public override bool CanPick(PhysicalItem physicalItem, int costThreshold)
        {
            if (Sleeper.Sleeping || Unit.Distance2D(physicalItem) > 400)
            {
                return false;
            }

            switch (physicalItem.Item.Id)
            {
                case AbilityId.item_gem:
                case AbilityId.item_rapier:
                {
                    return Unit.Inventory.FreeInventorySlots.Any();
                }
                case AbilityId.item_aegis:
                {
                    return false;
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
            return !Sleeper.Sleeping && Unit.Distance2D(rune) < 400;
        }
    }
}
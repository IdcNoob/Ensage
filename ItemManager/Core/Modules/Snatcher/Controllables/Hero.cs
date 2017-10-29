namespace ItemManager.Core.Modules.Snatcher.Controllables
{
    using System.Linq;

    using Ensage;
    using Ensage.SDK.Extensions;

    using Utils;

    internal class Hero : Controllable
    {
        public Hero(Unit unit)
            : base(unit)
        {
        }

        public override bool CanPick(PhysicalItem physicalItem, int costThreshold)
        {
            if (!CanPick() || Unit.Distance2D(physicalItem) > 400)
            {
                return false;
            }

            switch (physicalItem.Item.Id)
            {
                case AbilityId.item_gem:
                case AbilityId.item_rapier:
                case AbilityId.item_aegis:
                {
                    if (Unit.Inventory.FreeInventorySlots.Any())
                    {
                        return true;
                    }

                    if (!Unit.Inventory.FreeBackpackSlots.Any())
                    {
                        return false;
                    }

                    var item = Unit.Inventory.Items.OrderBy(x => x.Cost)
                        .FirstOrDefault(x => x.CanBeMovedToBackpack() && x.Cost < costThreshold);

                    if (item == null)
                    {
                        return false;
                    }

                    item.MoveItem(Unit.Inventory.FreeBackpackSlots.First());
                    return true;
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
            return CanPick() && Unit.Distance2D(rune) < 400;
        }

        private bool CanPick()
        {
            return !Sleeper.Sleeping && !Unit.HasModifier(ModifierUtils.SpiritBreakerCharge) && !Unit.IsChanneling();
        }
    }
}
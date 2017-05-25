namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_orchid)]
    [AbilityBasedModule(AbilityId.item_bloodthorn)]
    internal class OrchidMalevolence : OffensiveItemBase
    {
        public OrchidMalevolence(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Orchid malevolence", abilityId.ToString());
        }
    }
}
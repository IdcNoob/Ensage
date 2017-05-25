namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_ethereal_blade)]
    internal class EtherealBlade : OffensiveItemBase
    {
        public EtherealBlade(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Ethereal blade", abilityId.ToString());
        }
    }
}
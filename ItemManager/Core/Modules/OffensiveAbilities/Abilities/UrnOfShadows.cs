namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_urn_of_shadows)]
    internal class UrnOfShadows : OffensiveItemBase
    {
        public UrnOfShadows(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Urn of shadows", abilityId.ToString());
        }
    }
}
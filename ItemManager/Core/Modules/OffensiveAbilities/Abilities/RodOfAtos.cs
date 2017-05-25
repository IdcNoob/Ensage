namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_rod_of_atos)]
    internal class RodOfAtos : OffensiveItemBase
    {
        public RodOfAtos(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Rod of atos", abilityId.ToString());
        }
    }
}
namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_diffusal_blade)]
    [AbilityBasedModule(AbilityId.item_diffusal_blade_2)]
    internal class DiffusalBlade : OffensiveItemBase
    {
        public DiffusalBlade(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Diffusal blade", abilityId.ToString());
        }
    }
}
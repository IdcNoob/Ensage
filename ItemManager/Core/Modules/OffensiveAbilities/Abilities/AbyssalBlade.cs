namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_abyssal_blade)]
    internal class AbyssalBlade : OffensiveItemBase
    {
        public AbyssalBlade(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Abyssal blade", abilityId.ToString());
        }
    }
}
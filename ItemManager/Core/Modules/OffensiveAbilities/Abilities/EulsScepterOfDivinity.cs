namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_cyclone)]
    internal class EulsScepterOfDivinity : OffensiveItemBase
    {
        public EulsScepterOfDivinity(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Euls scepter of divinity", abilityId.ToString());
        }
    }
}
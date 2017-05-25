namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_sheepstick)]
    internal class ScytheOfVyse : OffensiveItemBase
    {
        public ScytheOfVyse(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Scythe of vyse", abilityId.ToString());
        }
    }
}
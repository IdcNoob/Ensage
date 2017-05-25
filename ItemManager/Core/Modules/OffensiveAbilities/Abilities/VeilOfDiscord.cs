namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_veil_of_discord)]
    internal class VeilOfDiscord : OffensiveItemBase
    {
        public VeilOfDiscord(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Veil of discord", abilityId.ToString());
        }
    }
}
namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_heavens_halberd)]
    internal class HeavensHalberd : OffensiveItemBase
    {
        public HeavensHalberd(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Heavens halberd", abilityId.ToString());
        }
    }
}
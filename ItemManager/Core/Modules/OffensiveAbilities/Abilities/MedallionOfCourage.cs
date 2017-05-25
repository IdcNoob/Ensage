namespace ItemManager.Core.Modules.OffensiveAbilities.Abilities
{
    using Attributes;

    using Ensage;

    using Menus;

    [AbilityBasedModule(AbilityId.item_medallion_of_courage)]
    [AbilityBasedModule(AbilityId.item_solar_crest)]
    internal class MedallionOfCourage : OffensiveItemBase
    {
        public MedallionOfCourage(Manager manager, MenuManager menu, AbilityId abilityId)
            : base(manager, menu, abilityId)
        {
            Menu = menu.OffensiveItemsMenu.CreateMenu("Medallion of courage", abilityId.ToString());
        }
    }
}
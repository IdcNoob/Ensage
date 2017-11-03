namespace ItemManager.Core.Abilities
{
    using Attributes;

    using Ensage;

    using Menus.Modules.Recovery;

    [Ability(AbilityId.item_spirit_vessel)]
    internal class SpiritVessel : UrnOfShadows
    {
        private readonly Item spiritVessel;

        public SpiritVessel(Ability ability, Manager manager)
            : base(ability, manager)
        {
            spiritVessel = ability as Item;
        }

        public override bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            return missingHealth >= menu.ItemSettingsMenu.SpiritVessel.HpThreshold;
        }
    }
}
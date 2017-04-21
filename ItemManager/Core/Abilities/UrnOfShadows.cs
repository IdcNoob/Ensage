namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Menus.Modules.Recovery;

    using Utils;

    [Ability(AbilityId.item_urn_of_shadows)]
    internal class UrnOfShadows : UsableAbility, IRecoveryAbility
    {
        private readonly Item urnOfShadows;

        public UrnOfShadows(Ability ability, Manager manager)
            : base(ability, manager)
        {
            urnOfShadows = ability as Item;

            ManaRestore = 0;
            HealthRestore = ability.AbilitySpecialData.First(x => x.Name == "soul_heal_amount").Value;

            PowerTreadsAttribute = Attribute.Agility;
            RestoredStats = RestoredStats.Health;
        }

        public float HealthRestore { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }

        public RestoredStats RestoredStats { get; }

        public override bool CanBeCasted()
        {
            return base.CanBeCasted() && urnOfShadows.CurrentCharges > 0
                   && !Manager.MyHero.HasModifier(ModifierUtils.UrnRegeneration);
        }

        public bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            return missingHealth >= menu.ItemSettingsMenu.UrnOfShadows.HpThreshold;
        }
    }
}
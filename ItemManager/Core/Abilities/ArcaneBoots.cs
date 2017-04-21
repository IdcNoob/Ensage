namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Menus.Modules.Recovery;

    using Utils;

    [Ability(AbilityId.item_arcane_boots)]
    internal class ArcaneBoots : UsableAbility, IRecoveryAbility
    {
        public ArcaneBoots(Ability ability, Manager manager)
            : base(ability, manager)
        {
            HealthRestore = 0;
            ManaRestore = ability.AbilitySpecialData.First(x => x.Name == "replenish_amount").Value;

            PowerTreadsAttribute = Attribute.Agility;
            RestoredStats = RestoredStats.Mana;
        }

        public float HealthRestore { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }

        public RestoredStats RestoredStats { get; }

        public bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            return missingMana >= menu.ItemSettingsMenu.ArcaneBootsSettings.MpThreshold;
        }
    }
}
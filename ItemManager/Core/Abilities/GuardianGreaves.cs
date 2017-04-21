namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Menus.Modules.Recovery;

    using Utils;

    [Ability(AbilityId.item_guardian_greaves)]
    internal class GuardianGreaves : UsableAbility, IRecoveryAbility
    {
        public GuardianGreaves(Ability ability, Manager manager)
            : base(ability, manager)
        {
            ManaRestore = ability.AbilitySpecialData.First(x => x.Name == "replenish_mana").Value;
            HealthRestore = ability.AbilitySpecialData.First(x => x.Name == "replenish_health").Value;

            PowerTreadsAttribute = Attribute.Agility;
            RestoredStats = RestoredStats.All;
        }

        public float HealthRestore { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }

        public RestoredStats RestoredStats { get; }

        public bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            return missingMana >= menu.ItemSettingsMenu.GuardianGreaves.MpThreshold
                   || missingHealth >= menu.ItemSettingsMenu.GuardianGreaves.HpThreshold;
        }
    }
}
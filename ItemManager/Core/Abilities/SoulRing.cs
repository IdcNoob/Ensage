namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Menus.Modules.Recovery;

    using Utils;

    [Ability(AbilityId.item_soul_ring)]
    internal class SoulRing : UsableAbility, IRecoveryAbility
    {
        public SoulRing(Ability ability, Manager manager)
            : base(ability, manager)
        {
            HealthRestore = -ability.AbilitySpecialData.First(x => x.Name == "health_sacrifice").Value;
            ManaRestore = ability.AbilitySpecialData.First(x => x.Name == "mana_gain").Value;

            PowerTreadsAttribute = Attribute.Strength;
            RestoredStats = RestoredStats.Mana;
        }

        public float HealthRestore { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }

        public RestoredStats RestoredStats { get; }

        public bool ShouldBeUsed(MyHero hero, RecoveryMenu menu, float missingHealth, float missingMana)
        {
            return missingMana >= menu.ItemSettingsMenu.SoulRing.MpThreshold
                   && hero.HealthPercentage >= menu.ItemSettingsMenu.SoulRing.HpThreshold;
        }
    }
}
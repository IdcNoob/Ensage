namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

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
            ItemRestoredStats = ItemUtils.Stats.All;
        }

        public float HealthRestore { get; }

        public ItemUtils.Stats ItemRestoredStats { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }
    }
}
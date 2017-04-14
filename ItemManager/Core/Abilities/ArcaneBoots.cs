namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

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
            ItemRestoredStats = ItemUtils.Stats.Mana;
        }

        public float HealthRestore { get; }

        public ItemUtils.Stats ItemRestoredStats { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }
    }
}
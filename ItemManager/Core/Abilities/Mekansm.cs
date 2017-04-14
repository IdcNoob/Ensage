namespace ItemManager.Core.Abilities
{
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Utils;

    [Ability(AbilityId.item_mekansm)]
    internal class Mekansm : UsableAbility, IRecoveryAbility
    {
        public Mekansm(Ability ability, Manager manager)
            : base(ability, manager)
        {
            ManaRestore = 0;
            HealthRestore = ability.AbilitySpecialData.First(x => x.Name == "heal_amount").Value;

            PowerTreadsAttribute = Attribute.Intelligence;
            ItemRestoredStats = ItemUtils.Stats.Health;
        }

        public float HealthRestore { get; }

        public ItemUtils.Stats ItemRestoredStats { get; }

        public float ManaRestore { get; }

        public Attribute PowerTreadsAttribute { get; }
    }
}
namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.skywrath_mage_arcane_bolt)]
    internal class ArcaneBolt : DefaultActive
    {
        private readonly float damageIncrease;

        public ArcaneBolt(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "bolt_damage").GetValue(i);
            }
            damageIncrease = Ability.AbilitySpecialData.First(x => x.Name == "int_multiplier").Value;
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            return (float)Math.Round(
                target.SpellDamageTaken(Damage[Level - 1] + (source.TotalIntelligence * damageIncrease), DamageType, source, Name));
        }
    }
}
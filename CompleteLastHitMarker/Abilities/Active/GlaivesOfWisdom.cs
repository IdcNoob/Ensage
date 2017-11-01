namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.silencer_glaives_of_wisdom)]
    internal class GlaivesOfWisdom : DefaultActive
    {
        public GlaivesOfWisdom(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "intellect_damage_pct").GetValue(i) / 100;
            }
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            return (float)Math.Round(target.SpellDamageTaken(Damage[Level - 1] * source.TotalIntelligence, DamageType, source, Name));
        }
    }
}
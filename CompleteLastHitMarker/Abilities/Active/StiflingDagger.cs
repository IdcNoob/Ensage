namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.phantom_assassin_stifling_dagger)]
    internal class StiflingDagger : DefaultActive
    {
        private readonly float baseDamage;

        private readonly float[] damagePenalty = new float[4];

        public StiflingDagger(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < ability.MaximumLevel; i++)
            {
                damagePenalty[i] = ability.AbilitySpecialData.First(x => x.Name == "attack_factor").GetValue(i) / 100
                                   + 1;
            }

            baseDamage = ability.AbilitySpecialData.First(x => x.Name == "base_damage").Value;
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            return (float)Math.Round(
                target.DamageTaken(
                    baseDamage + (source.MinimumDamage + source.BonusDamage) * damagePenalty[Level - 1],
                    DamageType,
                    source));
        }
    }
}
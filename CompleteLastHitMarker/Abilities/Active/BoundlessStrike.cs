namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.monkey_king_boundless_strike)]
    internal class BoundlessStrike : DefaultActive
    {
        public BoundlessStrike(Ability ability)
            : base(ability)
        {
            DealsAutoAttackDamage = true;

            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "strike_crit_mult").GetValue(i) / 100;
            }
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            var sourceDamage = source.MinimumDamage + source.BonusDamage;

            return (float)Math.Round(target.DamageTaken((Damage[Level - 1] * sourceDamage) - sourceDamage, DamageType, source));
        }
    }
}
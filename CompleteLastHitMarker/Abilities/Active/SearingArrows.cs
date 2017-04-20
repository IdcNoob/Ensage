namespace CompleteLastHitMarker.Abilities.Active
{
    using System;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.clinkz_searing_arrows)]
    internal class SearingArrows : DefaultActive
    {
        public SearingArrows(Ability ability)
            : base(ability)
        {
            DealsDamageToTowers = true;

            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "damage_bonus").GetValue(i);
            }
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            return (float)Math.Round(target.DamageTaken(Damage[Level - 1], DamageType, source));
        }
    }
}
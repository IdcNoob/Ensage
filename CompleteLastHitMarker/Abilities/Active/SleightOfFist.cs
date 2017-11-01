namespace CompleteLastHitMarker.Abilities.Active
{
    using System;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions.Damage;

    [Ability(AbilityId.ember_spirit_sleight_of_fist)]
    internal class SleightOfFist : DefaultActive
    {
        public SleightOfFist(Ability ability)
            : base(ability)
        {
        }

        public override float CalculateDamage(Hero source, Unit target)
        {
            return (float)Math.Round(target.DamageTaken(((float)source.MinimumDamage + source.BonusDamage) / 2, DamageType, source));
        }
    }
}
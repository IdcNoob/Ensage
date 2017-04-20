namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using Units.Base;

    [Ability(AbilityId.kunkka_tidebringer)]
    internal class Tidebringer : DefaultPassive
    {
        public Tidebringer(Ability ability)
            : base(ability)
        {
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            // fix for quell damage stacking
            return 0;
        }

        public override bool IsValid(Hero hero, KillableUnit unit)
        {
            return base.IsValid(hero, unit) & Ability.CanBeCasted();
        }
    }
}
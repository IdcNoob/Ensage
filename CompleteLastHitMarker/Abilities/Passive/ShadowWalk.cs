namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Units.Base;

    [Ability(AbilityId.bounty_hunter_wind_walk)]
    internal class ShadowWalk : DefaultPassive
    {
        private const string ShadowWalkModifier = "modifier_bounty_hunter_wind_walk";

        public ShadowWalk(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "bonus_damage").GetValue(i);
            }
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            return Damage[Level - 1];
        }

        public override bool IsValid(Hero hero, KillableUnit unit)
        {
            return base.IsValid(hero, unit) && hero.Modifiers.Any(x => x.Name == ShadowWalkModifier);
        }
    }
}
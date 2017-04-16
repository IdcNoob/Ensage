namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Units.Base;

    [Ability(AbilityId.storm_spirit_overload)]
    internal class Overload : DefaultPassive
    {
        private const string OverloadModifier = "modifier_storm_spirit_overload";

        public Overload(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < ability.MaximumLevel; i++)
            {
                Damage[i] = ability.GetDamage(i);
            }
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            return Damage[Ability.Level - 1];
        }

        public override bool IsValid(Hero hero, KillableUnit unit)
        {
            return base.IsValid(hero, unit) && hero.Modifiers.Any(x => x.Name == OverloadModifier);
        }
    }
}
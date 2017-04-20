namespace CompleteLastHitMarker.Abilities.Passive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Units.Base;

    using Utils;

    [Ability(AbilityId.antimage_mana_break)]
    internal class ManaBreak : DefaultPassive
    {
        private readonly float multiplier;

        public ManaBreak(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < ability.MaximumLevel; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "mana_per_hit").GetValue(i);
            }

            multiplier = Ability.AbilitySpecialData.First(x => x.Name == "damage_per_burn").Value;
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            if (unit.UnitType == UnitType.Tower)
            {
                return 0;
            }

            return Math.Min(Damage[Ability.Level - 1], unit.Mana) * multiplier;
        }
    }
}
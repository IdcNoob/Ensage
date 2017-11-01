namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Units.Base;

    [Ability(AbilityId.ursa_fury_swipes)]
    internal class FurySwipes : DefaultPassive
    {
        private const string FurySwipesModifier = "modifier_ursa_fury_swipes_damage_increase";

        public FurySwipes(Ability ability)
            : base(ability)
        {
            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "damage_per_stack").GetValue(i);
            }
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            return (unit.Modifiers.FirstOrDefault(x => x.Name == FurySwipesModifier)?.StackCount ?? 0) * Damage[Level - 1];
        }
    }
}
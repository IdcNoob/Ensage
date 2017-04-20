namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using Units.Base;

    [Ability(AbilityId.monkey_king_jingu_mastery)]
    internal class JinguMastery : DefaultPassive
    {
        private const string JinguMasteryModifier = "modifier_monkey_king_quadruple_tap_bonuses";

        public JinguMastery(Ability ability)
            : base(ability)
        {
            DamageType = DamageType.Physical;
            DealsDamageToTowers = true;
            DealsDamageToAllies = true;

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

            return Damage[Ability.Level - 1];
        }

        public override bool IsValid(Hero hero, KillableUnit unit)
        {
            return base.IsValid(hero, unit) && hero.HasModifier(JinguMasteryModifier);
        }
    }
}
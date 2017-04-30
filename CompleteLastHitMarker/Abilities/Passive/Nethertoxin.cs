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

    [Ability(AbilityId.viper_nethertoxin)]
    internal class Nethertoxin : DefaultPassive
    {
        private readonly float reduction;

        public Nethertoxin(Ability ability)
            : base(ability)
        {
            DealsDamageToTowers = true;

            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "bonus_damage").GetValue(i);
            }
            reduction = ability.AbilitySpecialData.First(x => x.Name == "non_hero_damage_pct").Value / 100;
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            return (float)(Damage[Level - 1] * Math.Pow(2, Math.Floor((1 - unit.HealthPercentage) / 0.2)) * reduction);
        }
    }
}
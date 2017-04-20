namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Units.Base;

    [Ability(AbilityId.life_stealer_feast)]
    internal class Feast : DefaultPassive
    {
        public Feast(Ability ability)
            : base(ability)
        {
            DamageType = DamageType.Physical;

            for (var i = 0u; i < Damage.Length; i++)
            {
                Damage[i] = Ability.AbilitySpecialData.First(x => x.Name == "hp_leech_percent").GetValue(i) / 100;
            }
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            return unit.Health * Damage[Level - 1];
        }
    }
}
namespace CompleteLastHitMarker.Abilities.Passive
{
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Base;

    using Ensage;

    using Interfaces;

    using Units.Base;

    [Ability(AbilityId.item_bfury)]
    internal class BattleFury : DefaultPassive
    {
        private readonly float meleeBonus;

        private readonly float rangedBonus;

        public BattleFury(Ability ability)
            : base(ability)
        {
            DamageType = DamageType.Physical;

            meleeBonus = ability.AbilitySpecialData.First(x => x.Name == "quelling_bonus").Value / 100;
            rangedBonus = ability.AbilitySpecialData.First(x => x.Name == "quelling_bonus_ranged").Value / 100;
        }

        public override float GetBonusDamage(Hero hero, KillableUnit unit, IEnumerable<IPassiveAbility> abilities)
        {
            if (!CanDoDamage(hero, unit, abilities))
            {
                return 0;
            }

            var minimumDamage = hero.MinimumDamage;
            return (hero.IsMelee ? minimumDamage * meleeBonus : minimumDamage * rangedBonus) - minimumDamage;
        }
    }
}
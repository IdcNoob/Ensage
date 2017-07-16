namespace Evader.UsableAbilities.Abilities
{
    using System.Linq;

    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using AbilityType = Data.AbilityType;

    internal class TimeWalk : BlinkAbility
    {
        private readonly float bonus;

        private readonly Ability talent;

        public TimeWalk(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            talent = Hero.GetAbilityById(AbilityId.special_bonus_unique_faceless_void);
            if (talent != null)
            {
                bonus = talent.AbilitySpecialData.First(x => x.Name == "value").Value;
            }
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + (talent?.Level > 0 ? bonus : 0);
        }
    }
}
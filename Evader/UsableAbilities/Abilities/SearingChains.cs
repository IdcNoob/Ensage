namespace Evader.UsableAbilities.Abilities
{
    using System.Linq;

    using Base;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using AbilityType = Core.AbilityType;

    internal class SearingChains : NotTargetable
    {
        #region Constructors and Destructors

        public SearingChains(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(Unit unit)
        {
            // todo: fix ally check

            if (
                ObjectManager.GetEntities<Unit>()
                    .Count(
                        x =>
                        x.IsValid && x.Team != HeroTeam && x.IsAlive && !x.IsMagicImmune()
                        && x.Distance2D(Hero) <= GetCastRange()) >= 3)
            {
                // < 66%
                return false;
            }

            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanCast()
                   && Hero.Distance2D(unit) <= GetCastRange() && CheckEnemy(unit);
        }

        #endregion
    }
}
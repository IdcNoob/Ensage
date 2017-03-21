namespace Evader.UsableAbilities.Abilities
{
    using System.Linq;

    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class SearingChains : NotTargetable
    {
        #region Constructors and Destructors

        public SearingChains(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            // todo: fix ally check

            if (
                ObjectManager.GetEntitiesParallel<Unit>()
                    .Count(
                        x =>
                            x.IsValid && x.IsAlive && x.IsSpawned && x.Team != HeroTeam && !x.IsMagicImmune()
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
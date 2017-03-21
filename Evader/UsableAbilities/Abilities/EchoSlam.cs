namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class EchoSlam : NotTargetable
    {
        #region Fields

        private readonly float aftershockRadius;

        #endregion

        #region Constructors and Destructors

        public EchoSlam(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            aftershockRadius = Hero.FindSpell("earthshaker_aftershock").GetRadius();
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            // todo: fix ally check

            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanCast()
                   && Hero.Distance2D(unit) <= aftershockRadius && CheckEnemy(unit);
        }

        #endregion
    }
}
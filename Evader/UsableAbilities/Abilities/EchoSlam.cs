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
        private readonly float aftershockRadius;

        public EchoSlam(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            aftershockRadius = Hero.FindSpell("earthshaker_aftershock").GetRadius();
        }

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            // todo: fix ally check

            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanCast()
                   && Hero.Distance2D(unit) <= aftershockRadius && CheckEnemy(unit);
        }
    }
}
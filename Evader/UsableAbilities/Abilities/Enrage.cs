namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class Enrage : NotTargetable
    {
        public Enrage(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.Distance2D(unit) <= GetCastRange()
                   && CheckEnemy(unit) && Hero.AghanimState();
        }
    }
}
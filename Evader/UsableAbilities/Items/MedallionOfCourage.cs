namespace Evader.UsableAbilities.Items
{
    using Ensage;

    using Evader.Core;
    using Evader.Data;
    using Evader.EvadableAbilities.Base;
    using Evader.UsableAbilities.Base;

    using AbilityType = Evader.Data.AbilityType;

    internal class MedallionOfCourage : Targetable
    {
        public MedallionOfCourage(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            if (Variables.Hero.Equals(unit))
            {
                return false;
            }

            return base.CanBeCasted(ability, unit);
        }
    }
}
namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class BlindingLight : Targetable
    {
        #region Constructors and Destructors

        public BlindingLight(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return base.CanBeCasted(ability, unit) && Ability.IsActivated;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(target.InFront(300));
            Sleep();
        }

        #endregion
    }
}
namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class SleightOfFist : UsableAbility
    {
        #region Constructors and Destructors

        //todo: improve
        public SleightOfFist(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return (float)Hero.GetTurnTime(unit) * 1.35f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(target.NetworkPosition);
            Sleep();
        }

        #endregion
    }
}
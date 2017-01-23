namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class ElectricVortex : UsableAbility
    {
        #region Constructors and Destructors

        public ElectricVortex(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            if (Hero.AghanimState())
            {
                return CastPoint;
            }
            return CastPoint + (unit.Equals(Hero) ? 0 : (float)Hero.GetTurnTime(unit) * 1.35f);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.AghanimState())
            {
                Ability.UseAbility();
            }
            else
            {
                Ability.UseAbility(target);
            }

            Sleep();
        }

        #endregion
    }
}
namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class Supernova : UsableAbility
    {
        #region Constructors and Destructors

        public Supernova(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return CastPoint;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.AghanimState())
            {
                Ability.UseAbility(Hero);
            }
            else
            {
                Ability.UseAbility();
            }
            Sleep();
        }

        #endregion
    }
}
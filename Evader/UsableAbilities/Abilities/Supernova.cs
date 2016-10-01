namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class Supernova : UsableAbility
    {
        #region Constructors and Destructors

        public Supernova(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
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
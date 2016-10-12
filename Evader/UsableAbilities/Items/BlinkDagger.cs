namespace Evader.UsableAbilities.Items
{
    using Base;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using AbilityType = Core.AbilityType;

    internal class BlinkDagger : BlinkAbility
    {
        #region Constructors and Destructors

        public BlinkDagger(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public void UseInFront()
        {
            Ability.UseAbility(Hero.InFront(1150));
            Sleep();
        }

        #endregion
    }
}
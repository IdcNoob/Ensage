namespace Evader.UsableAbilities.Items
{
    using Base;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class ForceStaff : BlinkAbility
    {
        #region Constructors and Destructors

        public ForceStaff(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.GetTurnTime(target) > 0)
            {
                Hero.Move(Hero.Position.Extend(target.Position, 40));
                Ability.UseAbility(Hero, true);
            }
            else
            {
                Ability.UseAbility(Hero);
            }

            Sleep();
        }

        #endregion
    }
}
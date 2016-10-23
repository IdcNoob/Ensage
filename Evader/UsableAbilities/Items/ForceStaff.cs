namespace Evader.UsableAbilities.Items
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class ForceStaff : BlinkAbility
    {
        #region Constructors and Destructors

        public ForceStaff(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
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
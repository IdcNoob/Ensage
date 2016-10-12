namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class Leap : BlinkAbility
    {
        #region Constructors and Destructors

        public Leap(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return CastPoint + (float)Hero.GetTurnTime(unit) + 0.15f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.GetTurnTime(target) > 0)
            {
                Hero.Move(Hero.Position.Extend(target.Position, 40));
                Ability.UseAbility(true);
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
namespace Evader.UsableAbilities.Base
{
    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using Utils;

    using AbilityType = Core.AbilityType;

    internal class Targetable : UsableAbility
    {
        #region Constructors and Destructors

        public Targetable(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return CastPoint + (unit.Equals(Hero) ? 0 : (float)Hero.GetTurnTime(unit));
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Ability.IsAbilityBehavior(AbilityBehavior.UnitTarget))
            {
                Ability.UseAbility(target);
            }
            else
            {
                Ability.UseAbility(target.Position);
            }

            Sleep();
        }

        #endregion
    }
}
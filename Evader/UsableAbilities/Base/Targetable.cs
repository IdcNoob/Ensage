namespace Evader.UsableAbilities.Base
{
    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class Targetable : UsableAbility
    {
        #region Constructors and Destructors

        public Targetable(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return CastPoint + (unit.Equals(Hero) ? 0 : (float)Hero.GetTurnTime(unit) * 1.35f);
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
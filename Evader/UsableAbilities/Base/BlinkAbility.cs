namespace Evader.UsableAbilities.Base
{
    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class BlinkAbility : UsableAbility
    {
        #region Constructors and Destructors

        public BlinkAbility(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return CastPoint + (float)Hero.GetTurnTime(unit) * 1.35f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(Hero.NetworkPosition.Extend(target.Position, GetCastRange() - 60));
            Sleep();
        }

        #endregion
    }
}
namespace Evader.UsableAbilities.Base
{
    using Core;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using Utils;

    using AbilityType = Core.AbilityType;

    internal class BlinkAbility : UsableAbility
    {
        #region Constructors and Destructors

        public BlinkAbility(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && (IsItem || Hero.CanCast());
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return CastPoint + (float)Hero.GetTurnTime(unit) * 1.5f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(Hero.NetworkPosition.Extend(target.Position, GetCastRange() - 50));
            Sleep();
        }

        #endregion
    }
}
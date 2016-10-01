namespace Evader.UsableAbilities.Base
{
    using Core;

    using Ensage;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class NotTargetable : UsableAbility
    {
        #region Constructors and Destructors

        public NotTargetable(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Properties

        protected bool DisjointsProjectile => AbilityFlags.HasFlag(AbilityFlags.DisjointsProjectile);

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return CastPoint;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility();
            Sleep();
        }

        #endregion
    }
}
namespace Evader.UsableAbilities.Items
{
    using Base;

    using Data;

    using Ensage;

    using AbilityType = Data.AbilityType;

    internal class HurricanePike : Targetable
    {
        public HurricanePike(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            CastPoint = 0.1f; // prevent late usage
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() / 2;
        }
    }
}
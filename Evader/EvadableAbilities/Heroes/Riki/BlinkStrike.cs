namespace Evader.EvadableAbilities.Heroes.Riki
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class BlinkStrike : NoObstacleAbility
    {
        #region Constructors and Destructors

        public BlinkStrike(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        #endregion
    }
}
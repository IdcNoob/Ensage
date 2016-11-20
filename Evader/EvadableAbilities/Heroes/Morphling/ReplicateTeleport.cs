namespace Evader.EvadableAbilities.Heroes.Morphling
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ReplicateTeleport : NoObstacleAbility
    {
        #region Constructors and Destructors

        public ReplicateTeleport(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.Morphling
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ReplicateTeleport : NoObstacleAbility
    {
        public ReplicateTeleport(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }
    }
}
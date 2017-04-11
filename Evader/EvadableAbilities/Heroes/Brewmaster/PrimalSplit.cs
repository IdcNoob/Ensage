namespace Evader.EvadableAbilities.Heroes.Brewmaster
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class PrimalSplit : NoObstacleAbility
    {
        public PrimalSplit(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }
    }
}
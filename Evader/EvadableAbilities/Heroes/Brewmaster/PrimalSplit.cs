namespace Evader.EvadableAbilities.Heroes.Brewmaster
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class PrimalSplit : NoObstacleAbility
    {
        #region Constructors and Destructors

        public PrimalSplit(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        #endregion
    }
}
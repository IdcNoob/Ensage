namespace Evader.EvadableAbilities.Heroes.Underlord
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class DarkRift : NoObstacleAbility
    {
        #region Constructors and Destructors

        public DarkRift(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.Chen
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class HandOfGod : NoObstacleAbility
    {
        #region Constructors and Destructors

        public HandOfGod(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        #endregion
    }
}
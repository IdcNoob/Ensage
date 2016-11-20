namespace Evader.EvadableAbilities.Heroes.Omniknight
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class GuardianAngel : NoObstacleAbility
    {
        #region Constructors and Destructors

        public GuardianAngel(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        #endregion
    }
}
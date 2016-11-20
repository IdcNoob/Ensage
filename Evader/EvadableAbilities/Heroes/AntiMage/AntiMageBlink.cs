namespace Evader.EvadableAbilities.Heroes.AntiMage
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class AntiMageBlink : NoObstacleAbility
    {
        #region Constructors and Destructors

        public AntiMageBlink(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.FacelessVoid
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class TimeWalk : NoObstacleAbility
    {
        #region Constructors and Destructors

        public TimeWalk(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
            DisableAbilities.Remove(Eul);
        }

        #endregion
    }
}
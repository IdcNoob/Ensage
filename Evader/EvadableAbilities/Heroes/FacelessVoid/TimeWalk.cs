namespace Evader.EvadableAbilities.Heroes.FacelessVoid
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class TimeWalk : NoObstacleAbility
    {
        public TimeWalk(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
            DisableAbilities.Remove(Eul);
        }
    }
}
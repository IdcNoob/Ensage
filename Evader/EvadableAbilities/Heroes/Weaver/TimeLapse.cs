namespace Evader.EvadableAbilities.Heroes.Weaver
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class TimeLapse : NoObstacleAbility
    {
        public TimeLapse(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);

            DisableAbilities.Remove(Eul);
        }
    }
}
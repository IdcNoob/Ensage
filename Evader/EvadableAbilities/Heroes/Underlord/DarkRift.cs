namespace Evader.EvadableAbilities.Heroes.Underlord
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class DarkRift : NoObstacleAbility
    {
        public DarkRift(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }
    }
}
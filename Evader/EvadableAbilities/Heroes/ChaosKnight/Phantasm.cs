namespace Evader.EvadableAbilities.Heroes.ChaosKnight
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Phantasm : NoObstacleAbility
    {
        public Phantasm(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }
    }
}
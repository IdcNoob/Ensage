namespace Evader.EvadableAbilities.Heroes.AntiMage
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class AntiMageBlink : NoObstacleAbility
    {
        public AntiMageBlink(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }
    }
}
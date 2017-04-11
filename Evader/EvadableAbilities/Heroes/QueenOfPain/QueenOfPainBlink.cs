namespace Evader.EvadableAbilities.Heroes.QueenOfPain
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class QueenOfPainBlink : NoObstacleAbility
    {
        public QueenOfPainBlink(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }
    }
}
namespace Evader.EvadableAbilities.Heroes.Mirana
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class MoonlightShadow : NoObstacleAbility
    {
        public MoonlightShadow(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }
    }
}
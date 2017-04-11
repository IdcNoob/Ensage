namespace Evader.EvadableAbilities.Heroes.NagaSiren
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class SongOfTheSiren : AOE
    {
        public SongOfTheSiren(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);
        }
    }
}
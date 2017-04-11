namespace Evader.EvadableAbilities.Heroes.Underlord
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class PitOfMalice : LinearAOE, IModifier
    {
        public PitOfMalice(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(BallLightning);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(FortunesEnd);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.Add(Eul);
            Modifier.AllyCounterAbilities.AddRange(AllyPurges);
        }

        public EvadableModifier Modifier { get; }
    }
}
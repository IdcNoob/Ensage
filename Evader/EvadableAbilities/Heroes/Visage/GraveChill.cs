namespace Evader.EvadableAbilities.Heroes.Visage
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class GraveChill : LinearTarget, IModifier
    {
        public GraveChill(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(AphoticShield);
            Modifier.AllyCounterAbilities.Add(FortunesEnd);

            CounterAbilities.Add(PhaseShift);
        }

        public EvadableModifier Modifier { get; }
    }
}
namespace Evader.EvadableAbilities.Heroes.VengefulSpirit
{
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class WaveOfTerror : LinearProjectile, IModifier
    {
        public WaveOfTerror(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(AphoticShield);
            Modifier.AllyCounterAbilities.Add(FortunesEnd);
        }

        public EvadableModifier Modifier { get; }
    }
}
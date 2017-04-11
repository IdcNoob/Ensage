namespace Evader.EvadableAbilities.Heroes.Brewmaster
{
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class DrunkenHaze : Projectile, IModifier
    {
        public DrunkenHaze(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(AphoticShield);
            Modifier.AllyCounterAbilities.Add(FortunesEnd);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.Add(Eul);
        }

        public EvadableModifier Modifier { get; }

        public override float GetProjectileSpeed()
        {
            return 1600;
        }
    }
}
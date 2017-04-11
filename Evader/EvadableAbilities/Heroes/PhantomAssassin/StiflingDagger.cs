namespace Evader.EvadableAbilities.Heroes.PhantomAssassin
{
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class StiflingDagger : Projectile, IModifier
    {
        public StiflingDagger(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);

            CounterAbilities.Remove(BladeMail);

            Modifier.AllyCounterAbilities.Add(Lotus);
        }

        public EvadableModifier Modifier { get; }
    }
}
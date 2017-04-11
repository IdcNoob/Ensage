namespace Evader.EvadableAbilities.Items
{
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class RodOfAtos : Projectile, IModifier
    {
        public RodOfAtos(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(BallLightning);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(FortunesEnd);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.Add(Eul);
            Modifier.AllyCounterAbilities.AddRange(AllyPurges);
        }

        public EvadableModifier Modifier { get; }

        public override float GetProjectileSpeed()
        {
            return 1500;
        }
    }
}
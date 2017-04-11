namespace Evader.EvadableAbilities.Heroes.Oracle
{
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class FortunesEnd : Projectile, IModifier
    {
        public FortunesEnd(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(AphoticShield);

            AdditionalDelay = Ability.GetChannelTime(0);
        }

        public EvadableModifier Modifier { get; }
    }
}
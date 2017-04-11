namespace Evader.EvadableAbilities.Heroes.Necrophos
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class ReapersScythe : LinearTarget, IModifier
    {
        public ReapersScythe(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(NetherWard);
            CounterAbilities.AddRange(Invis);

            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.Add(FalsePromise);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
        }

        public EvadableModifier Modifier { get; }
    }
}
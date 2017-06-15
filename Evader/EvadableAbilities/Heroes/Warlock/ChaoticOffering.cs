namespace Evader.EvadableAbilities.Heroes.Warlock
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class ChaoticOffering : LinearAOE, IModifier
    {
        public ChaoticOffering(Ability ability)
            : base(ability)
        {
            DisableTimeSinceCastCheck = true;

            Modifier = new EvadableModifier(
                HeroTeam,
                EvadableModifier.GetHeroType.LowestHealth,
                ignoreRemainingTime: true);

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        public EvadableModifier Modifier { get; }
    }
}
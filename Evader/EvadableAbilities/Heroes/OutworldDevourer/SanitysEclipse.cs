namespace Evader.EvadableAbilities.Heroes.OutworldDevourer
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class SanitysEclipse : LinearAOE
    {
        public SanitysEclipse(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
            BlinkAbilities.AddRange(BlinkAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
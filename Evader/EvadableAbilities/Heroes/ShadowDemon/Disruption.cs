namespace Evader.EvadableAbilities.Heroes.ShadowDemon
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Disruption : LinearTarget
    {
        public Disruption(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Lotus);
        }
    }
}
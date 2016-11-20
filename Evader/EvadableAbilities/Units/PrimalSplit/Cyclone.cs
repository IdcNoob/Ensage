namespace Evader.EvadableAbilities.Units.PrimalSplit
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Cyclone : LinearTarget
    {
        #region Constructors and Destructors

        public Cyclone(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.Tusk
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class WalrusKick : LinearTarget
    {
        #region Constructors and Destructors

        public WalrusKick(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(SnowBall);
        }

        #endregion
    }
}
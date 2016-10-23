namespace Evader.EvadableAbilities.Heroes.Pudge
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Dismember : LinearTarget
    {
        #region Constructors and Destructors

        public Dismember(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(Invis);
        }

        #endregion
    }
}
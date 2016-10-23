namespace Evader.EvadableAbilities.Heroes.Warlock
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class FatalBonds : LinearAOE
    {
        #region Constructors and Destructors

        public FatalBonds(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
        }

        #endregion
    }
}
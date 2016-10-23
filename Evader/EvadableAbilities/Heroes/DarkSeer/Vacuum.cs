namespace Evader.EvadableAbilities.Heroes.DarkSeer
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Vacuum : LinearAOE
    {
        #region Constructors and Destructors

        public Vacuum(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
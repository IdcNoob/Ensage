namespace Evader.EvadableAbilities.Heroes.WitchDoctor
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Maledict : LinearAOE
    {
        #region Constructors and Destructors

        public Maledict(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
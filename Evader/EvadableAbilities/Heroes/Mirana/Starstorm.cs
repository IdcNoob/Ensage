namespace Evader.EvadableAbilities.Heroes.Mirana
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Starstorm : AOE
    {
        #region Constructors and Destructors

        public Starstorm(Ability ability)
            : base(ability)
        {
            //todo add aghs ?

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
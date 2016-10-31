namespace Evader.EvadableAbilities.Heroes.Timbersaw
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Chakram : LinearProjectile
    {
        #region Constructors and Destructors

        public Chakram(Ability ability)
            : base(ability)
        {
            //todo can be improved
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        #endregion
    }
}
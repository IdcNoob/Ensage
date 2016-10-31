namespace Evader.EvadableAbilities.Heroes.Timbersaw
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class TimberChain : LinearProjectile
    {
        #region Constructors and Destructors

        public TimberChain(Ability ability)
            : base(ability)
        {
            //todo check tree + hit time + correct speed for chain & timber

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        #endregion
    }
}
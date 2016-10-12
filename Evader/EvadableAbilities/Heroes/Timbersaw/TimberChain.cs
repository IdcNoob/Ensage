namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal sealed class TimberChain : LinearProjectile
    {
        #region Constructors and Destructors

        public TimberChain(Ability ability)
            : base(ability)
        {
            //todo check tree + hit time + correct speed for chain & timber

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

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
        }

        #endregion
    }
}
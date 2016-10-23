namespace Evader.EvadableAbilities.Heroes.VengefulSpirit
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class WaveOfTerror : LinearProjectile
    {
        #region Constructors and Destructors

        public WaveOfTerror(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
        }

        #endregion
    }
}
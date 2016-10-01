namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class WaveOfTerror : LinearProjectile
    {
        #region Constructors and Destructors

        public WaveOfTerror(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_vengefulspirit_wave_of_terror";

            CounterAbilities.Add(PhaseShift);
        }

        #endregion
    }
}
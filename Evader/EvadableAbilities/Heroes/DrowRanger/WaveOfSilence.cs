namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class WaveOfSilence : LinearProjectile
    {
        #region Constructors and Destructors

        public WaveOfSilence(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
        }

        #endregion

        #region Public Methods and Operators

        public override float GetProjectileSpeed()
        {
            return base.GetProjectileSpeed() + 200;
        }

        #endregion

        #region Methods

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 100;
        }

        #endregion
    }
}
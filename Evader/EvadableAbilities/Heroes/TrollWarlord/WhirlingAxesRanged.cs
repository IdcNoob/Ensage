namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class WhirlingAxesRanged : LinearProjectile
    {
        #region Constructors and Destructors

        public WhirlingAxesRanged(Ability ability)
            : base(ability)
        {
            IgnorePathfinder = true;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
        }

        #endregion

        #region Methods

        protected override float GetEndRadius()
        {
            return 300;
        }

        #endregion
    }
}
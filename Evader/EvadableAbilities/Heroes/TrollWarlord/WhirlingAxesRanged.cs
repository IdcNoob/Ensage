namespace Evader.EvadableAbilities.Heroes.TrollWarlord
{
    using Ensage;

    using static Data.AbilityNames;

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
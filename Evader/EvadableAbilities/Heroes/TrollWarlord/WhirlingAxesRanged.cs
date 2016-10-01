namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class WhirlingAxesRanged : LinearDynamicProjectile
    {
        #region Constructors and Destructors

        public WhirlingAxesRanged(Ability ability)
            : base(ability)
        {
            IgnorePathfinder = true;
            EndWidth = 300;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
        }

        #endregion
    }
}
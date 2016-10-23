namespace Evader.EvadableAbilities.Heroes.ShadowDemon
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class ShadowPoison : LinearProjectile
    {
        #region Constructors and Destructors

        public ShadowPoison(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
        }

        #endregion
    }
}
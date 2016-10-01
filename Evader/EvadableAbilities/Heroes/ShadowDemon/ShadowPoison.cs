namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class ShadowPoison : LinearProjectile
    {
        #region Constructors and Destructors

        public ShadowPoison(Ability ability)
            : base(ability)
        {
            Radius += 20;

            CounterAbilities.Add(PhaseShift);
        }

        #endregion
    }
}
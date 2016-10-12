namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class Shockwave : LinearProjectile
    {
        #region Constructors and Destructors

        public Shockwave(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return base.GetRadius() + 60;
        }

        #endregion
    }
}
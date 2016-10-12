namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class DualBreath : LinearProjectile
    {
        #region Constructors and Destructors

        public DualBreath(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
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
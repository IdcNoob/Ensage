namespace Evader.EvadableAbilities.Heroes.Jakiro
{
    using Ensage;

    using static Data.AbilityNames;

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
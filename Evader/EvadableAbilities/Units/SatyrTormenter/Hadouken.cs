namespace Evader.EvadableAbilities.Units.SatyrTormenter
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Hadouken : LinearProjectile
    {
        #region Constructors and Destructors

        public Hadouken(Ability ability)
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
            return base.GetCastRange() + 400;
        }

        #endregion
    }
}
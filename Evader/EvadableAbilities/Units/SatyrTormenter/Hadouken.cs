namespace Evader.EvadableAbilities.Units.SatyrTormenter
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Hadouken : LinearProjectile
    {
        public Hadouken(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 400;
        }
    }
}
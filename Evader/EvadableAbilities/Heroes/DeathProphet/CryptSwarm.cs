namespace Evader.EvadableAbilities.Heroes.DeathProphet
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class CryptSwarm : LinearProjectile
    {
        public CryptSwarm(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + GetEndRadius();
        }
    }
}
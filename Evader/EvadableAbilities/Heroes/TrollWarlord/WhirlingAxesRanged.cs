namespace Evader.EvadableAbilities.Heroes.TrollWarlord
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class WhirlingAxesRanged : LinearProjectile
    {
        public WhirlingAxesRanged(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        protected override float GetEndRadius()
        {
            return 300;
        }
    }
}
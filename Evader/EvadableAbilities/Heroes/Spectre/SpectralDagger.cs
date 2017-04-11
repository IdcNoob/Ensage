namespace Evader.EvadableAbilities.Heroes.Spectre
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class SpectralDagger : Projectile
    {
        public SpectralDagger(Ability ability)
            : base(ability)
        {
            IsDisjointable = false;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
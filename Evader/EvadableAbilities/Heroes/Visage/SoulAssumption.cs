namespace Evader.EvadableAbilities.Heroes.Visage
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class SoulAssumption : Projectile
    {
        public SoulAssumption(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Lotus);
        }
    }
}
namespace Evader.EvadableAbilities.Heroes.Tinker
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class HeatSeekingMissile : Projectile
    {
        //todo fix multi targets ?
        public HeatSeekingMissile(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
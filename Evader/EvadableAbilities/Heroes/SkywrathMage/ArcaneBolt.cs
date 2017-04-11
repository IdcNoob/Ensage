namespace Evader.EvadableAbilities.Heroes.SkywrathMage
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class ArcaneBolt : Projectile
    {
        public ArcaneBolt(Ability ability)
            : base(ability)
        {
            IsDisjointable = false;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
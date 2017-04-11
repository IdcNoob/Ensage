namespace Evader.EvadableAbilities.Heroes.Mirana
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Starstorm : AOE
    {
        public Starstorm(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
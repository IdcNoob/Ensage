namespace Evader.EvadableAbilities.Heroes.Chen
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class TestOfFaith : LinearTarget
    {
        public TestOfFaith(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
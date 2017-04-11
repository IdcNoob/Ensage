namespace Evader.EvadableAbilities.Heroes.Oracle
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class PurifyingFlames : LinearTarget
    {
        public PurifyingFlames(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
        }
    }
}
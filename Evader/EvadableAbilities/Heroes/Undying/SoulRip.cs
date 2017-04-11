namespace Evader.EvadableAbilities.Heroes.Undying
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class SoulRip : LinearTarget
    {
        public SoulRip(Ability ability)
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
namespace Evader.EvadableAbilities.Heroes.NightStalker
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Void : LinearTarget
    {
        public Void(Ability ability)
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
namespace Evader.EvadableAbilities.Heroes.Lich
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class FrostBlast : LinearTarget
    {
        public FrostBlast(Ability ability)
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
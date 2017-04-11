namespace Evader.EvadableAbilities.Heroes.ShadowShaman
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class EtherShock : LinearAOE
    {
        public EtherShock(Ability ability)
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
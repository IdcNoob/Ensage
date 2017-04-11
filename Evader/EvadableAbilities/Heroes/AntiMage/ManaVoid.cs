namespace Evader.EvadableAbilities.Heroes.AntiMage
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ManaVoid : LinearAOE
    {
        public ManaVoid(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(Invis);
        }
    }
}
namespace Evader.EvadableAbilities.Units.HellbearSmasher
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class HellbearThunderClap : AOE
    {
        public HellbearThunderClap(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
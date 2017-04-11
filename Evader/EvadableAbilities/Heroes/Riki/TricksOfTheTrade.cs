namespace Evader.EvadableAbilities.Heroes.Riki
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class TricksOfTheTrade : AOE
    {
        public TricksOfTheTrade(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
        }
    }
}
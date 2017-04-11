namespace Evader.EvadableAbilities.Heroes.LegionCommander
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class OverwhelmingOdds : LinearAOE
    {
        public OverwhelmingOdds(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
namespace Evader.EvadableAbilities.Heroes.Undying
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Decay : LinearAOE
    {
        public Decay(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
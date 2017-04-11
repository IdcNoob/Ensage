namespace Evader.EvadableAbilities.Heroes.Dazzle
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ShadowWave : LinearAOE
    {
        public ShadowWave(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.Add("item_ghost");
            CounterAbilities.Add("item_buckler");
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
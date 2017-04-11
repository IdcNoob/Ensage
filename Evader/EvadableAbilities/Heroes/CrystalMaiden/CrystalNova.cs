namespace Evader.EvadableAbilities.Heroes.CrystalMaiden
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class CrystalNova : LinearAOE
    {
        public CrystalNova(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }
    }
}
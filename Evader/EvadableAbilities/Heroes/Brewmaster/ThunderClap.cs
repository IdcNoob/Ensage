namespace Evader.EvadableAbilities.Heroes.Brewmaster
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ThunderClap : AOE
    {
        #region Constructors and Destructors

        public ThunderClap(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
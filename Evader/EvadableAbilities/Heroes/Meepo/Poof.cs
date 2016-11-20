namespace Evader.EvadableAbilities.Heroes.Meepo
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Poof : AOE
    {
        #region Constructors and Destructors

        public Poof(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        #endregion
    }
}
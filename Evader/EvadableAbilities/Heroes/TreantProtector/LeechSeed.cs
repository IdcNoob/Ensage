namespace Evader.EvadableAbilities.Heroes.TreantProtector
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class LeechSeed : LinearTarget
    {
        #region Constructors and Destructors

        public LeechSeed(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
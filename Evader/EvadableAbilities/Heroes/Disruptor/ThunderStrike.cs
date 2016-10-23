namespace Evader.EvadableAbilities.Heroes.Disruptor
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ThunderStrike : LinearTarget
    {
        #region Constructors and Destructors

        public ThunderStrike(Ability ability)
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
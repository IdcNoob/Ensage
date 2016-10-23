namespace Evader.EvadableAbilities.Heroes.Undying
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class SoulRip : LinearTarget
    {
        #region Constructors and Destructors

        public SoulRip(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
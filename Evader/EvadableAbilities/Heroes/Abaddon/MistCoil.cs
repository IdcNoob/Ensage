namespace Evader.EvadableAbilities.Heroes.Abaddon
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class MistCoil : LinearTarget
    {
        #region Constructors and Destructors

        public MistCoil(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
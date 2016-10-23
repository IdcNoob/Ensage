namespace Evader.EvadableAbilities.Heroes.NyxAssassin
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ManaBurn : LinearTarget
    {
        #region Constructors and Destructors

        public ManaBurn(Ability ability)
            : base(ability)
        {
            //todo: fix aghs

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
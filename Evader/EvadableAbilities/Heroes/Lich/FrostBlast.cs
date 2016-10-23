namespace Evader.EvadableAbilities.Heroes.Lich
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class FrostBlast : LinearTarget
    {
        #region Constructors and Destructors

        public FrostBlast(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
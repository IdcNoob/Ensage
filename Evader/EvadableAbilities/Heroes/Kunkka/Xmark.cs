namespace Evader.EvadableAbilities.Heroes.Kunkka
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class Xmark : LinearTarget
    {
        #region Constructors and Destructors

        public Xmark(Ability ability)
            : base(ability)
        {
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.Visage
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class GraveChill : LinearTarget
    {
        #region Constructors and Destructors

        public GraveChill(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
        }

        #endregion
    }
}
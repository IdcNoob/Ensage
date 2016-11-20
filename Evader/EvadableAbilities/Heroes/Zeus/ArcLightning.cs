namespace Evader.EvadableAbilities.Heroes.Zeus
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ArcLightning : LinearAOE
    {
        #region Constructors and Destructors

        public ArcLightning(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            CounterAbilities.Add(PhaseShift);
        }

        #endregion
    }
}
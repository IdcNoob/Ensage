namespace Evader.EvadableAbilities.Heroes.Silencer
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class ArcaneCurse : LinearAOE
    {
        #region Constructors and Destructors

        public ArcaneCurse(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.Riki
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class SmokeScreen : LinearAOE
    {
        #region Constructors and Destructors

        public SmokeScreen(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
        }

        #endregion
    }
}
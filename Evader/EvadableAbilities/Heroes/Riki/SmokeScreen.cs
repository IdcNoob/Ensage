namespace Evader.EvadableAbilities.Heroes.Riki
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class SmokeScreen : LinearAOE
    {
        public SmokeScreen(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
        }
    }
}
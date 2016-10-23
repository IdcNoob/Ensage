namespace Evader.EvadableAbilities.Units.Familiar
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class StoneForm : AOE
    {
        #region Constructors and Destructors

        public StoneForm(Ability ability)
            : base(ability)
        {
            DisableAbilities.Clear();

            //todo fix visage familiars
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsLowDisable);
        }

        #endregion
    }
}
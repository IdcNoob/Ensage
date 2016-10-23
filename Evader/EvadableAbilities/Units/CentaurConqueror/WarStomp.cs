namespace Evader.EvadableAbilities.Units.CentaurConqueror
{
    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class WarStomp : AOE
    {
        #region Fields

        private readonly float radius;

        #endregion

        #region Constructors and Destructors

        public WarStomp(Ability ability)
            : base(ability)
        {
            DisableAbilities.Clear();

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsLowDisable);

            radius = ability.GetRadius() + 100;
        }

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return radius;
        }

        #endregion
    }
}
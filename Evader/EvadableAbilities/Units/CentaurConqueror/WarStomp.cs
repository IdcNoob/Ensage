namespace Evader.EvadableAbilities.Units.CentaurConqueror
{
    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class WarStomp : AOE
    {
        private readonly float radius;

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

        protected override float GetRadius()
        {
            return radius;
        }
    }
}
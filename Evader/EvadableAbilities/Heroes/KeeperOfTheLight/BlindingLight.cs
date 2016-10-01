namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class BlindingLight : LinearAOE
    {
        #region Constructors and Destructors

        public BlindingLight(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_keeper_of_the_light_blinding_light";

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsLowDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
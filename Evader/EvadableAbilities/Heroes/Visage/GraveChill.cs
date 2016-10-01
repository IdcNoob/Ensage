namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class GraveChill : LinearTarget
    {
        #region Constructors and Destructors

        public GraveChill(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_visage_grave_chill_debuff";

            CounterAbilities.Add(PhaseShift);
        }

        #endregion
    }
}
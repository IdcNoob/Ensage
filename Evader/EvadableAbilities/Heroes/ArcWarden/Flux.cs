namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class Flux : LinearTarget
    {
        #region Constructors and Destructors

        public Flux(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_arc_warden_flux";

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
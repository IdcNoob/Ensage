namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class Void : LinearTarget
    {
        #region Constructors and Destructors

        public Void(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_night_stalker_void";

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
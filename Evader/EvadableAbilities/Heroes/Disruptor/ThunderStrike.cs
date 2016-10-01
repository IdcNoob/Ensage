namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class ThunderStrike : LinearTarget
    {
        #region Constructors and Destructors

        public ThunderStrike(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_disruptor_thunder_strike";

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
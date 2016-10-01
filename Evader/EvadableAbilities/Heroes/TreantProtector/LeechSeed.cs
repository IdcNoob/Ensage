namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class LeechSeed : LinearTarget
    {
        #region Constructors and Destructors

        public LeechSeed(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_treant_leech_seed";

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
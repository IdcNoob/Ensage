namespace Evader.EvadableAbilities.Heroes.Luna
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class LucentBeam : LinearTarget
    {
        #region Constructors and Destructors

        public LucentBeam(Ability ability)
            : base(ability)
        {
            BlinkAbilities.Clear();
            DisableAbilities.Clear();

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
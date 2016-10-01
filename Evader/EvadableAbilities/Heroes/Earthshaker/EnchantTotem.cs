namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class EnchantTotem : AOE
    {
        #region Constructors and Destructors

        public EnchantTotem(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsLowDisable);
            CounterAbilities.AddRange(VsDamage);
        }

        #endregion
    }
}
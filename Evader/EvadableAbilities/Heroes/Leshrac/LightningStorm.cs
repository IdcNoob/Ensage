namespace Evader.EvadableAbilities.Heroes.Leshrac
{
    using Base;

    using Data;

    using Ensage;

    internal class LightningStorm : LinearAOE
    {
        #region Constructors and Destructors

        public LightningStorm(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(AbilityNames.PhaseShift);
            CounterAbilities.AddRange(AbilityNames.VsDamage);
            CounterAbilities.AddRange(AbilityNames.VsMagic);
            CounterAbilities.Add(AbilityNames.Armlet);
            CounterAbilities.Add(AbilityNames.Bloodstone);
        }

        #endregion
    }
}
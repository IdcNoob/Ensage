namespace Evader.EvadableAbilities.Heroes.Leshrac
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class LightningStorm : LinearAOE
    {
        #region Constructors and Destructors

        public LightningStorm(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        #endregion
    }
}
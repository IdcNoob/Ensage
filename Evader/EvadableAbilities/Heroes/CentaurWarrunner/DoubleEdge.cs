namespace Evader.EvadableAbilities.Heroes.CentaurWarrunner
{
    using Base;

    using Ensage;

    using static Data.AbilityNames;

    internal class DoubleEdge : LinearTarget
    {
        #region Constructors and Destructors

        public DoubleEdge(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
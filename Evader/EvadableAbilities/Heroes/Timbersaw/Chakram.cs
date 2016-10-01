namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class Chakram : LinearAOE
    {
        #region Constructors and Destructors

        public Chakram(Ability ability)
            : base(ability)
        {
            //todo add particle

            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
        }

        #endregion
    }
}
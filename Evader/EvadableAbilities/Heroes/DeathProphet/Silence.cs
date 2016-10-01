namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class Silence : LinearAOE
    {
        #region Constructors and Destructors

        public Silence(Ability ability)
            : base(ability)
        {
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
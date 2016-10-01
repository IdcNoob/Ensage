namespace Evader.EvadableAbilities.Heroes.Jakiro
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class Macropyre : LinearAOE
    {
        #region Constructors and Destructors

        public Macropyre(Ability ability)
            : base(ability)
        {
            //todo add stay time

            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
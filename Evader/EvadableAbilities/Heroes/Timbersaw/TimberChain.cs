namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class TimberChain : LinearAOE
    {
        #region Fields

        private readonly float radius;

        #endregion

        #region Constructors and Destructors

        public TimberChain(Ability ability)
            : base(ability)
        {
            //todo check tree + hit time

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);

            radius = Ability.AbilitySpecialData.First(x => x.Name == "radius").Value + 50;
        }

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return radius;
        }

        #endregion
    }
}
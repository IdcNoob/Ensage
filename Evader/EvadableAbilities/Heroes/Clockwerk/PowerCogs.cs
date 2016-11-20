namespace Evader.EvadableAbilities.Heroes.Clockwerk
{
    using Base;

    using Ensage;

    internal class PowerCogs : AOE
    {
        #region Constructors and Destructors

        public PowerCogs(Ability ability)
            : base(ability)
        {
            // only pathfinder
        }

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return base.GetRadius() + 200;
        }

        #endregion
    }
}
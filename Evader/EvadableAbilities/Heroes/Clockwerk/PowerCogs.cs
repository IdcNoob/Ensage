namespace Evader.EvadableAbilities.Heroes.Clockwerk
{
    using Base;

    using Ensage;

    internal class PowerCogs : AOE
    {
        public PowerCogs(Ability ability)
            : base(ability)
        {
            // only pathfinder
        }

        protected override float GetRadius()
        {
            return base.GetRadius() + 200;
        }
    }
}
namespace Evader.EvadableAbilities.Modifiers.Special
{
    using Ensage;

    internal class XmarkModifier : EvadableModifier
    {
        #region Constructors and Destructors

        public XmarkModifier(
            Team team,
            GetHeroType type,
            float maxRemainingTime = 0,
            int minStacks = 0,
            float maxDistanceToSource = 300)
            : base(team, type, maxRemainingTime, minStacks, maxDistanceToSource)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCountered()
        {
            return false;
        }

        #endregion
    }
}
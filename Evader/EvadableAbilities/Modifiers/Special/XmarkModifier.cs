namespace Evader.EvadableAbilities.Modifiers.Special
{
    using Ensage;

    internal class XmarkModifier : EvadableModifier
    {
        public XmarkModifier(
            Team team,
            GetHeroType type,
            float maxRemainingTime = 0,
            int minStacks = 0,
            float maxDistanceToSource = 300)
            : base(team, type, maxRemainingTime, minStacks, maxDistanceToSource)
        {
        }

        public override bool CanBeCountered()
        {
            return false;
        }
    }
}
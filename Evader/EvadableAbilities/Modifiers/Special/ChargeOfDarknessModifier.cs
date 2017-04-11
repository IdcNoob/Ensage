namespace Evader.EvadableAbilities.Modifiers.Special
{
    using Ensage;
    using Ensage.Common.Extensions;

    internal class ChargeOfDarknessModifier : EvadableModifier
    {
        private readonly Hero spiritBreaker;

        public ChargeOfDarknessModifier(
            Team team,
            GetHeroType type,
            Unit abilityOwner,
            float maxRemainingTime = 0,
            int minStacks = 0,
            float maxDistanceToSource = 300,
            bool ignoreRemainingTime = false)
            : base(team, type, maxRemainingTime, minStacks, maxDistanceToSource, ignoreRemainingTime)
        {
            spiritBreaker = (Hero)abilityOwner;
        }

        public override bool CanBeCountered()
        {
            if (!IsValid() || !spiritBreaker.IsVisible)
            {
                return false;
            }

            return spiritBreaker.Distance2D(GetSource()) <= 600;
        }

        public override Hero GetEnemyHero()
        {
            return spiritBreaker;
        }
    }
}
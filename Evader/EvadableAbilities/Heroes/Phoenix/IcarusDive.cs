namespace Evader.EvadableAbilities.Heroes.Phoenix
{
    using Base;

    using Ensage;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    internal class IcarusDive : NoObstacleAbility
    {
        public IcarusDive(Ability ability)
            : base(ability)
        {
            DisableAbilities.AddRange(DisableAbilityNames);
        }

        public override void Check()
        {
            if (StartCast <= 0 && Ability.IsHidden && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + 2;
                Obstacle = Handle;
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return Game.RawGameTime < StartCast + 1;
        }
    }
}
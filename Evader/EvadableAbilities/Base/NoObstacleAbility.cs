namespace Evader.EvadableAbilities.Base
{
    using Ensage;

    internal class NoObstacleAbility : EvadableAbility
    {
        #region Constructors and Destructors

        public NoObstacleAbility(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay;
                Obstacle = Handle;
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), AbilityOwner.Position);
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + CastPoint + AdditionalDelay - Game.RawGameTime;
        }

        #endregion
    }
}
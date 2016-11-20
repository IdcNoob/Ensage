namespace Evader.EvadableAbilities.Base
{
    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    internal abstract class LinearTarget : LinearAOE
    {
        #region Fields

        private readonly float radius;

        #endregion

        #region Constructors and Destructors

        protected LinearTarget(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;
            radius = 75;
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay;
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + 150);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
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
            AbilityDrawer.DrawRectangle(StartPosition, EndPosition, GetRadius());
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
namespace Evader.EvadableAbilities.Base
{
    using Ensage;
    using Ensage.Common.Extensions;

    using Utils;

    internal abstract class LinearTarget : LinearAOE
    {
        #region Fields

        private readonly float radius;

        #endregion

        #region Constructors and Destructors

        protected LinearTarget(Ability ability)
            : base(ability)
        {
            IgnorePathfinder = true;
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

        #endregion

        #region Methods

        protected override float GetRadius()
        {
            return radius;
        }

        #endregion
    }
}
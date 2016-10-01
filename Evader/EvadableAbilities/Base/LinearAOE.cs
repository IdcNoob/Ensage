namespace Evader.EvadableAbilities.Base
{
    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    using Utils;

    internal class LinearAOE : Linear
    {
        #region Fields

        private readonly float radius;

        #endregion

        #region Constructors and Destructors

        public LinearAOE(Ability ability)
            : base(ability)
        {
            radius = ability.GetRadius() + 50;
            Debugger.WriteLine("// Radius: " + radius);
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;
            var phase = IsInPhase;

            if (phase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                EndCast = StartCast + CastPoint;
            }
            else if (phase && Obstacle == null && (int)Owner.RotationDifference == 0)
            {
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
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

            Utils.DrawRectangle(StartPosition, EndPosition, GetRadius());

            Vector2 textPosition;
            Drawing.WorldToScreen(StartPosition, out textPosition);
            Drawing.DrawText(
                GetRemainingTime().ToString("0.00"),
                "Arial",
                textPosition,
                new Vector2(20),
                Color.White,
                FontFlags.None);
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + CastPoint - Game.RawGameTime;
        }

        #endregion

        #region Methods

        protected virtual float GetRadius()
        {
            return radius;
        }

        #endregion
    }
}
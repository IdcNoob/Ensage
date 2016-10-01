namespace Evader.EvadableAbilities.Base
{
    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    using Utils;

    internal class AOE : EvadableAbility
    {
        #region Fields

        private readonly float radius;

        #endregion

        #region Constructors and Destructors

        public AOE(Ability ability)
            : base(ability)
        {
            radius = ability.GetRadius() + 75;
            Debugger.WriteLine("// Radius: " + radius);
        }

        #endregion

        #region Properties

        protected Vector3 Position { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (Obstacle == null && IsInPhase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                Position = Owner.NetworkPosition;
                EndCast = StartCast + CastPoint;
                Obstacle = Pathfinder.AddObstacle(Position, GetRadius(), Obstacle);
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

            Vector2 textPosition;
            Drawing.WorldToScreen(Position, out textPosition);
            Drawing.DrawText(
                GetRemainingTime().ToString("0.00"),
                "Arial",
                textPosition,
                new Vector2(20),
                Color.White,
                FontFlags.None);

            if (Particle == null)
            {
                Particle = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", Position);
                Particle.SetControlPoint(1, new Vector3(255, 0, 0));
                Particle.SetControlPoint(2, new Vector3(radius * -1, 255, 0));
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
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
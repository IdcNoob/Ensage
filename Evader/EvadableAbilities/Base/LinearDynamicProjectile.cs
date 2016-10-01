namespace Evader.EvadableAbilities.Base
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using Utils;

    internal class LinearDynamicProjectile : Linear
    {
        #region Fields

        private readonly float speed;

        #endregion

        #region Constructors and Destructors

        public LinearDynamicProjectile(Ability ability)
            : base(ability)
        {
            speed = ability.GetProjectileSpeed();

            var widthString = AbilityDatabase.Find(ability.Name).Width;
            var endWidthString = AbilityDatabase.Find(ability.Name).EndWidth;

            StartWidth = ability.AbilitySpecialData.FirstOrDefault(x => x.Name == widthString)?.Value ?? 200;
            EndWidth = ability.AbilitySpecialData.FirstOrDefault(x => x.Name == endWidthString)?.Value
                       ?? StartWidth + 100;

            Debugger.WriteLine("// StartWidth: " + StartWidth);
            Debugger.WriteLine("// EndWidth: " + EndWidth);
            Debugger.WriteLine("// Speed: " + speed);
        }

        #endregion

        #region Properties

        protected float EndWidth { get; set; }

        protected float StartWidth { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;
            var phase = IsInPhase;

            if (phase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
            }
            else if (phase && Obstacle == null && (int)Owner.RotationDifference == 0)
            {
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange() + EndWidth);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, StartWidth, EndWidth, Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !phase)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetProjectileRadius(), EndWidth);
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            if (Particle == null)
            {
                Particle = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", StartPosition);
                Particle.SetControlPoint(1, new Vector3(255, 0, 0));
                Particle.SetControlPoint(2, new Vector3((EndWidth + StartWidth) / 2, 255, 0));
            }

            Utils.DrawRectangle(StartPosition, EndPosition, StartWidth, EndWidth);
            Vector2 textPosition;
            Drawing.WorldToScreen(StartPosition, out textPosition);
            Drawing.DrawText(
                GetRemainingTime().ToString("0.00"),
                "Arial",
                textPosition,
                new Vector2(20),
                Color.White,
                FontFlags.None);

            Particle?.SetControlPoint(0, GetProjectilePosition());
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.BasePredict(350);

            if (IsInPhase && position.Distance2D(StartPosition) < StartWidth)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + CastPoint
                   + (position.Distance2D(StartPosition) - GetProjectileRadius(position)) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        #endregion

        #region Methods

        protected virtual Vector3 GetProjectilePosition()
        {
            return IsInPhase
                       ? StartPosition
                       : StartPosition.Extend(
                           EndPosition,
                           (Game.RawGameTime - StartCast - CastPoint) * GetProjectileSpeed());
        }

        protected float GetProjectileRadius(Vector3 position = new Vector3())
        {
            if (position.IsZero)
            {
                position = GetProjectilePosition();
            }

            if (StartPosition.Distance2D(position) > GetCastRange())
            {
                return EndWidth;
            }

            return (EndWidth - StartWidth) * (StartPosition.Distance2D(position) / GetCastRange()) + StartWidth;
        }

        protected virtual float GetProjectileSpeed()
        {
            return speed;
        }

        #endregion
    }
}
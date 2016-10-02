namespace Evader.EvadableAbilities.Base
{
    using System;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using Utils;

    internal class LinearProjectile : Linear
    {
        #region Fields

        private readonly float speed;

        #endregion

        #region Constructors and Destructors

        public LinearProjectile(Ability ability)
            : base(ability)
        {
            speed = ability.GetProjectileSpeed();
            Radius = ability.GetRadius() + 25;

            Debugger.WriteLine("// Radius: " + Radius);
            Debugger.WriteLine("// Speed: " + speed);
        }

        #endregion

        #region Properties

        protected float Radius { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;
            var phase = IsInPhase;

            if (phase && StartCast + CastPoint <= time && time > EndCast)
            {
                StartCast = time;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
            }
            else if (phase && Obstacle == null && (int)Owner.RotationDifference == 0)
            {
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange() + Radius / 2);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, Radius, Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !phase)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), Radius);
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
                Particle.SetControlPoint(2, new Vector3(Radius, 255, 0));
            }

            Utils.DrawRectangle(StartPosition, EndPosition, Radius);
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

            var position = hero.NetworkPosition;

            if (IsInPhase && position.Distance2D(StartPosition) < Radius)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + CastPoint + (position.Distance2D(StartPosition) - Radius) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        #endregion

        #region Methods

        protected virtual Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return IsInPhase
                       ? StartPosition
                       : StartPosition.Extend(
                           EndPosition,
                           (Game.RawGameTime - StartCast - (ignoreCastPoint ? 0 : CastPoint)) * GetProjectileSpeed());
        }

        protected virtual float GetProjectileSpeed()
        {
            return speed;
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Base
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using Utils;

    internal class Projectile : LinearTarget
    {
        #region Fields

        private readonly float speed;

        private readonly float width;

        private bool fowCast;

        private bool projectileAdded;

        private Vector3 projectilePostion;

        private Hero projectileTarget;

        #endregion

        #region Constructors and Destructors

        public Projectile(Ability ability)
            : base(ability)
        {
            speed = ability.GetProjectileSpeed();
            IsDisjointable = true;
            width = 100;
            Debugger.WriteLine("// Speed: " + speed);
            Debugger.WriteLine("// Width: " + width);
        }

        #endregion

        #region Public Properties

        public bool IsDisjointable { get; protected set; }

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
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetWidth(), Obstacle);
            }
            else if (projectileTarget != null && Obstacle == null && !fowCast)
            {
                fowCast = true;
                StartCast = time;
                //EndCast = StartCast + GetProjectilePosition(fowCast).Distance2D(projectileTarget) / GetProjectileSpeed();
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                StartPosition = Owner.NetworkPosition;
                EndPosition = StartPosition.Extend(projectileTarget.Position, GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetWidth(), Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                if (!ProjectileLaunched())
                {
                    EndPosition = Owner.InFront(GetCastRange());
                    Pathfinder.UpdateObstacle(Obstacle.Value, StartPosition, EndPosition);
                }
                else if (projectileTarget != null)
                {
                    //    EndCast = time + GetProjectilePosition(fowCast).Distance2D(projectileTarget) / GetProjectileSpeed();
                    EndPosition = StartPosition.Extend(
                        projectileTarget.Position,
                        projectileTarget.Distance2D(StartPosition) + width);

                    Pathfinder.UpdateObstacle(
                        Obstacle.Value,
                        projectileTarget.NetworkPosition.Extend(StartPosition, width),
                        projectileTarget.NetworkPosition.Extend(EndPosition, width));
                    //Pathfinder.UpdateObstacle(
                    //    Obstacle.Value,
                    //    StartPosition,
                    //    projectileTarget.NetworkPosition.Extend(EndPosition, width));
                }
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            if (Particle == null && !GetProjectilePosition(fowCast).IsZero)
            {
                Particle = new ParticleEffect(@"particles\ui_mouseactions\drag_selected_ring.vpcf", StartPosition);
                Particle.SetControlPoint(1, new Vector3(255, 0, 0));
                Particle.SetControlPoint(2, new Vector3(GetWidth(), 255, 0));
            }

            Particle?.SetControlPoint(0, GetProjectilePosition(fowCast));

            if (!projectileAdded)

            {
                Utils.DrawRectangle(StartPosition, EndPosition, GetWidth());
            }

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

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();

            projectileTarget = null;
            projectilePostion = new Vector3();
            fowCast = false;
            projectileAdded = false;
        }

        public virtual float GetProjectileSpeed()
        {
            return speed;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            if (projectileTarget == null)
            {
                if (IsDisjointable)
                {
                    return StartCast + CastPoint - Game.RawGameTime + 0.2f;
                }

                return StartCast + CastPoint
                       + hero.NetworkPosition.Distance2D(GetProjectilePosition()) / GetProjectileSpeed()
                       - Game.RawGameTime;
            }

            return StartCast + (fowCast ? -0.1f : CastPoint)
                   + projectileTarget.NetworkPosition.Distance2D(GetProjectilePosition(fowCast)) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(float remainingTime = 0)
        {
            return IsDisjointable && ProjectileLaunched() && remainingTime > 0;
        }

        public override bool IsStopped()
        {
            var check = !IsInPhase && CanBeStopped() && !fowCast;

            if (check)
            {
                End();
            }

            return check;
        }

        public bool ProjectileLaunched()
        {
            return Game.RawGameTime >= StartCast + CastPoint || projectileAdded;
        }

        public void SetProjectile(Vector3 position, Hero target)
        {
            if (!projectileAdded)
            {
                projectileTarget = target;
                projectileAdded = true;
            }
            projectilePostion = position;
        }

        public float TimeSinceCast()
        {
            if (Ability.Level <= 0 || !Owner.IsVisible)
            {
                return float.MaxValue;
            }

            var cooldownLength = Ability.CooldownLength;

            if (cooldownLength <= 0)
            {
                return float.MaxValue;
            }

            return cooldownLength - Ability.Cooldown;
        }

        #endregion

        #region Methods

        //protected virtual Vector3 GetProjectilePosition()
        //{
        //    return IsInPhase ? StartPosition : projectilePostion;
        //}

        protected virtual Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return IsInPhase
                       ? StartPosition
                       : StartPosition.Extend(
                           EndPosition,
                           (Game.RawGameTime - StartCast - (ignoreCastPoint ? -0.2f : CastPoint)) * GetProjectileSpeed());
        }

        protected override float GetWidth()
        {
            return width;
        }

        #endregion
    }
}
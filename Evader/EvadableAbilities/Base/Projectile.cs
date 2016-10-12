namespace Evader.EvadableAbilities.Base
{
    using System;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using UsableAbilities.Base;

    using Utils;

    internal abstract class Projectile : LinearProjectile
    {
        #region Fields

        private readonly float radius;

        private bool fowCast;

        private bool projectileAdded;

        private Vector3 projectilePostion;

        private Hero projectileTarget;

        #endregion

        #region Constructors and Destructors

        protected Projectile(Ability ability)
            : base(ability)
        {
            IsDisjointable = true;
            radius = 75;
            IgnorePathfinder = true;
        }

        #endregion

        #region Public Properties

        public bool IsDisjointable { get; protected set; }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (projectileTarget != null && Obstacle == null && !fowCast)
            {
                fowCast = true;
                StartCast = Game.RawGameTime;
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = StartPosition.Extend(projectileTarget.Position, GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                if (!ProjectileLaunched())
                {
                    EndPosition = AbilityOwner.InFront(GetCastRange());
                    Pathfinder.UpdateObstacle(Obstacle.Value, StartPosition, EndPosition);
                    AbilityDrawer.UpdateRectaglePosition(StartPosition, EndPosition, GetRadius());
                }
                else if (projectileTarget != null)
                {
                    AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
                    //    EndCast = Game.RawGameTime + GetProjectilePosition(fowCast).Distance2D(projectileTarget) / GetProjectileSpeed();
                    EndPosition = StartPosition.Extend(
                        projectileTarget.Position,
                        projectileTarget.Distance2D(StartPosition) + GetRadius());
                    Pathfinder.UpdateObstacle(
                        Obstacle.Value,
                        projectileTarget.NetworkPosition.Extend(StartPosition, GetRadius()),
                        projectileTarget.NetworkPosition.Extend(EndPosition, GetRadius()));
                }
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            if (!ProjectileLaunched())
            {
                AbilityDrawer.DrawRectangle(StartPosition, EndPosition, GetRadius());
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);
            AbilityDrawer.DrawCircle(StartPosition, GetRadius());

            AbilityDrawer.UpdateCirclePosition(GetProjectilePosition());
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

        public override float GetRemainingDisableTime()
        {
            return fowCast ? 0 : base.GetRemainingDisableTime();
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = projectileTarget?.NetworkPosition ?? hero.NetworkPosition;

            if (position.Distance2D(AbilityOwner) < 250)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + (fowCast ? -0.1f : CastPoint)
                   + Math.Max(position.Distance2D(GetProjectilePosition(fowCast)) - 100 - GetRadius(), 0)
                   / GetProjectileSpeed() - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return IsDisjointable && ProjectileLaunched() && remainingTime > 0;
        }

        public override bool IsStopped()
        {
            return base.IsStopped() && !fowCast;
        }

        public bool ProjectileLaunched()
        {
            return Game.RawGameTime >= StartCast + CastPoint - 0.06f || projectileAdded;
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
            if (Ability.Level <= 0 || !AbilityOwner.IsVisible)
            {
                return float.MaxValue;
            }

            var cooldownLength = Ability.CooldownLength;
            return cooldownLength <= 0 ? float.MaxValue : cooldownLength - Ability.Cooldown;
        }

        #endregion

        #region Methods

        protected override float GetEndRadius()
        {
            return radius;
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return projectileAdded ? projectilePostion : StartPosition;
        }

        protected override float GetRadius()
        {
            return radius;
        }

        #endregion
    }
}
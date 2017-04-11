namespace Evader.EvadableAbilities.Base
{
    using System;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using UsableAbilities.Base;

    internal abstract class Projectile : LinearProjectile
    {
        private readonly float radius;

        protected Projectile(Ability ability)
            : base(ability)
        {
            IsDisjointable = true;
            radius = 75;
            DisablePathfinder = true;
        }

        public bool IsDisjointable { get; protected set; }

        protected bool FowCast { get; set; }

        protected Vector3 LastProjectilePosition { get; set; }

        protected bool ProjectileAdded { get; set; }

        protected Vector3 ProjectilePostion { get; set; }

        protected Hero ProjectileTarget { get; set; }

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
            else if (ProjectileTarget != null && Obstacle == null && !FowCast)
            {
                FowCast = true;
                StartCast = Game.RawGameTime;
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = StartPosition.Extend(ProjectileTarget.Position, GetCastRange());
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
                    AbilityDrawer.UpdateRectanglePosition(StartPosition, EndPosition, GetRadius());
                }
                else if (ProjectileTarget != null)
                {
                    var projectilePosition = GetProjectilePosition(FowCast);

                    if (projectilePosition == LastProjectilePosition)
                    {
                        End();
                        return;
                    }

                    LastProjectilePosition = projectilePosition;

                    AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
                    //    EndCast = Game.RawGameTime + GetProjectilePosition(fowCast).Distance2D(projectileTarget) / GetProjectileSpeed();
                    EndPosition = StartPosition.Extend(
                        ProjectileTarget.Position,
                        ProjectileTarget.Distance2D(StartPosition) + GetRadius());
                    Pathfinder.UpdateObstacle(
                        Obstacle.Value,
                        ProjectileTarget.NetworkPosition.Extend(StartPosition, GetRadius()),
                        ProjectileTarget.NetworkPosition.Extend(EndPosition, GetRadius()));
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

            ProjectileTarget = null;
            ProjectilePostion = new Vector3();
            FowCast = false;
            ProjectileAdded = false;
        }

        public override float GetRemainingDisableTime()
        {
            return FowCast ? 0 : base.GetRemainingDisableTime();
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = ProjectileTarget?.NetworkPosition ?? hero.NetworkPosition;

            if (position.Distance2D(AbilityOwner) < 250)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + (FowCast ? -0.1f : CastPoint)
                   + Math.Max(position.Distance2D(GetProjectilePosition(FowCast)) - 100 - GetRadius(), 0)
                   / GetProjectileSpeed() - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return IsDisjointable && ProjectileLaunched() && remainingTime > 0;
        }

        public override bool IsStopped()
        {
            return base.IsStopped() && !FowCast;
        }

        public bool ProjectileLaunched()
        {
            return Game.RawGameTime >= StartCast + CastPoint - 0.06f || ProjectileAdded;
        }

        public void SetProjectile(Vector3 position, Hero target)
        {
            if (!ProjectileAdded)
            {
                ProjectileTarget = target;
                ProjectileAdded = true;
            }
            ProjectilePostion = position;
        }

        protected override float GetEndRadius()
        {
            return radius;
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return ProjectileAdded ? ProjectilePostion : StartPosition;
        }

        protected override float GetRadius()
        {
            return radius;
        }
    }
}
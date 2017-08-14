namespace Evader.EvadableAbilities.Heroes.ArcWarden
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class SparkWraith : Projectile
    {
        private Vector3 lastProjectilePosition;

        public SparkWraith(Ability ability)
            : base(ability)
        {
            IsDisjointable = false;

            CounterAbilities.Add(Eul);
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (ProjectileTarget != null && Obstacle == null)
            {
                StartCast = Game.RawGameTime;
                StartPosition = ProjectilePostion;
                EndPosition = ProjectileTarget.Position;
                EndCast = StartCast + ProjectileTarget.Distance2D(EndPosition) / GetProjectileSpeed();
                Obstacle = Pathfinder.AddObstacle(EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                if (ProjectileTarget == null)
                {
                    return;
                }

                var projectilePosition = GetProjectilePosition();
                if (projectilePosition == lastProjectilePosition)
                {
                    End();
                    return;
                }

                lastProjectilePosition = projectilePosition;

                EndCast = Game.RawGameTime
                          + (ProjectileTarget.Distance2D(projectilePosition) - 20) / GetProjectileSpeed();
                EndPosition = StartPosition.Extend(
                    ProjectileTarget.Position,
                    ProjectileTarget.Distance2D(StartPosition) + GetRadius());
                Pathfinder.UpdateObstacle(
                    Obstacle.Value,
                    ProjectileTarget.NetworkPosition.Extend(StartPosition, GetRadius()),
                    ProjectileTarget.NetworkPosition.Extend(EndPosition, GetRadius()));
            }
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = ProjectileTarget?.NetworkPosition ?? hero.NetworkPosition;

            return position.Distance2D(GetProjectilePosition()) / GetProjectileSpeed() - 0.2f;
        }
    }
}
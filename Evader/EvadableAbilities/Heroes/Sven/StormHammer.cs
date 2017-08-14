namespace Evader.EvadableAbilities.Heroes.Sven
{
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Modifiers;

    using SharpDX;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class StormHammer : Projectile, IModifier
    {
        private readonly float stunRadius;

        private Vector3 lastProjectilePosition;

        private bool obstacleToAOE;

        public StormHammer(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);

            stunRadius = Ability.GetRadius() + 100;
        }

        public EvadableModifier Modifier { get; }

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
                    if (projectilePosition == lastProjectilePosition)
                    {
                        End();
                        return;
                    }

                    lastProjectilePosition = projectilePosition;

                    AbilityDrawer.Dispose(AbilityDrawer.Type.Rectangle);
                    EndCast = Game.RawGameTime
                              + (ProjectileTarget.Distance2D(projectilePosition) - 20) / GetProjectileSpeed();

                    if (!obstacleToAOE)
                    {
                        Obstacle = Pathfinder.AddObstacle(ProjectileTarget.Position, stunRadius, Obstacle);
                        obstacleToAOE = true;
                    }
                    else
                    {
                        Pathfinder.UpdateObstacle(Obstacle.Value, ProjectileTarget.Position, stunRadius);
                    }
                }
            }
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();

            obstacleToAOE = false;
        }
    }
}
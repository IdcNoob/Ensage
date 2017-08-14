namespace Evader.EvadableAbilities.Heroes.Tidehunter
{
    using System;
    using System.Linq;

    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Modifiers;

    using SharpDX;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class Gush : Projectile, IModifier
    {
        private readonly float aghanimProjectileRadius;

        private readonly float aghanimProjectileSpeed;

        public Gush(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);

            aghanimProjectileSpeed = ability.AbilitySpecialData.First(x => x.Name == "speed_scepter").Value;
            aghanimProjectileRadius = ability.AbilitySpecialData.First(x => x.Name == "aoe_scepter").Value + 50;
        }

        public EvadableModifier Modifier { get; }

        public override void Check()
        {
            if (AbilityOwner.AghanimState())
            {
                if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
                {
                    StartCast = Game.RawGameTime;
                    EndCast = StartCast + CastPoint + AdditionalDelay + GetCastRange() / GetProjectileSpeed();
                }
                else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
                {
                    StartPosition = AbilityOwner.NetworkPosition;
                    EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 2);
                    Obstacle = Pathfinder.AddObstacle(
                        StartPosition,
                        EndPosition,
                        GetRadius(),
                        GetEndRadius(),
                        Obstacle);
                }
                else if (StartCast > 0 && Game.RawGameTime > EndCast)
                {
                    End();
                }
                else if (Obstacle != null && !CanBeStopped())
                {
                    Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius(), GetEndRadius());
                }
            }
            else
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
        }

        public override float GetProjectileSpeed()
        {
            return AbilityOwner.AghanimState() ? aghanimProjectileSpeed : base.GetProjectileSpeed();
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = ProjectileTarget?.NetworkPosition ?? hero.NetworkPosition;

            if (position.Distance2D(AbilityOwner) <= GetRadius())
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            if (AbilityOwner.AghanimState())
            {
                return StartCast + CastPoint
                       + (position.Distance2D(StartPosition) - GetProjectileRadius(position) - 60)
                       / GetProjectileSpeed() - Game.RawGameTime;
            }

            return StartCast + (FowCast ? -0.1f : CastPoint)
                   + Math.Max(position.Distance2D(GetProjectilePosition(FowCast)) - 100 - GetRadius(), 0)
                   / GetProjectileSpeed() - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return !AbilityOwner.AghanimState() && base.IgnoreRemainingTime(ability, remainingTime);
        }

        protected override float GetEndRadius()
        {
            return GetRadius();
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            if (AbilityOwner.AghanimState())
            {
                return IsInPhase
                           ? StartPosition
                           : StartPosition.Extend(
                               EndPosition,
                               (Game.RawGameTime - StartCast - (ignoreCastPoint ? 0 : CastPoint))
                               * GetProjectileSpeed());
            }

            return ProjectileAdded ? ProjectilePostion : StartPosition;
        }

        protected override float GetRadius()
        {
            return AbilityOwner.AghanimState() ? aghanimProjectileRadius : base.GetRadius();
        }
    }
}
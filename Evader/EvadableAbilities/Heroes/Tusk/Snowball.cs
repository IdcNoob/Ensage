namespace Evader.EvadableAbilities.Heroes.Tusk
{
    using System.Linq;

    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Modifiers;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class Snowball : Projectile, IModifier
    {
        public Snowball(Ability ability)
            : base(ability)
        {
            IsDisjointable = false;

            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);

            AdditionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "snowball_windup").Value;
        }

        public EvadableModifier Modifier { get; }

        public override void Check()
        {
            if (ProjectileTarget != null && Obstacle == null && !FowCast)
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
}
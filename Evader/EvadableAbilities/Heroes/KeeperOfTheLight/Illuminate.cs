namespace Evader.EvadableAbilities.Heroes.KeeperOfTheLight
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Illuminate : LinearProjectile, IParticle
    {
        public Illuminate(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (particleArgs.Name.Contains("charge"))
            {
                DelayAction.Add(1, () => StartPosition = particleArgs.ParticleEffect.GetControlPoint(0).SetZ(350));
            }
            else
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();

                DelayAction.Add(
                    1,
                    () =>
                        {
                            var position = particleArgs.ParticleEffect.GetControlPoint(0).SetZ(350);
                            StartPosition = StartPosition.Extend(position, 150);
                            EndPosition = position.Extend(StartPosition, GetCastRange());
                            Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
                        });
            }
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Obstacle != null && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius(), GetEndRadius());
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);
            AbilityDrawer.DrawArcRectangle(StartPosition, EndPosition, GetRadius(), GetEndRadius());
            AbilityDrawer.DrawCircle(StartPosition, (GetRadius() + GetEndRadius()) / 2);

            AbilityDrawer.UpdateCirclePosition(GetProjectilePosition());
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.NetworkPosition;

            if (IsInPhase && position.Distance2D(StartPosition) <= GetRadius())
            {
                return StartCast + CastPoint + AdditionalDelay - Game.RawGameTime;
            }

            return StartCast + (position.Distance2D(StartPosition) - GetProjectileRadius(position))
                   / GetProjectileSpeed() - Game.RawGameTime;
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return StartPosition.Extend(EndPosition, (Game.RawGameTime - StartCast) * GetProjectileSpeed());
        }

        protected override float GetRadius()
        {
            return base.GetRadius() + 50;
        }
    }
}
namespace Evader.EvadableAbilities.Heroes
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class MeatHook : LinearProjectile, IParticle
    {
        #region Fields

        private bool particleAdded;

        private ParticleEffect particleEffect;

        #endregion

        #region Constructors and Destructors

        public MeatHook(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.AddRange(Invis);
        }

        #endregion

        #region Public Methods and Operators

        public void AddParticle(ParticleEffect particle)
        {
            particleEffect = particle;
        }

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
                EndPosition = Owner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, Radius, Obstacle);
            }
            else if (particleEffect != null && particleEffect.IsValid && !particleAdded && Obstacle == null)
            {
                particleAdded = true;
                StartPosition = particleEffect.GetControlPoint(0);
                EndPosition = StartPosition.Extend(particleEffect.GetControlPoint(1), GetCastRange() + Radius / 2);
                StartCast = time - 0.1f;
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, Radius, Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !phase)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(particleAdded), EndPosition);
            }
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();

            particleEffect = null;
            particleAdded = false;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            if (particleAdded)
            {
                return StartCast + (hero.NetworkPosition.Distance2D(StartPosition) - Radius) / GetProjectileSpeed()
                       - Game.RawGameTime;
            }

            if (IsInPhase && hero.NetworkPosition.Distance2D(StartPosition) <= Radius)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + CastPoint
                   + (hero.NetworkPosition.Distance2D(StartPosition) - Radius) / GetProjectileSpeed() - Game.RawGameTime;
        }

        public override bool IsStopped()
        {
            var check = !IsInPhase && CanBeStopped() && !particleAdded;

            if (check)
            {
                End();
            }

            return check;
        }

        #endregion

        #region Methods

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            // only for drawings
            return base.GetProjectilePosition(particleAdded);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using Utils;

    using static Core.Abilities;

    internal class CryptSwarm : LinearDynamicProjectile, IParticle
    {
        #region Fields

        private bool particleAdded;

        private ParticleEffect particleEffect;

        #endregion

        #region Constructors and Destructors

        public CryptSwarm(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
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
                EndPosition = Owner.InFront(GetCastRange() + EndWidth);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, StartWidth, EndWidth, Obstacle);
            }
            else if (particleEffect != null && particleEffect.IsValid && !particleAdded && Obstacle == null)
            {
                StartCast = time;
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                particleAdded = true;
                StartPosition = particleEffect.GetControlPoint(0);
                EndPosition = StartPosition.Extend(particleEffect.GetControlPoint(3), GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, StartWidth, EndWidth, Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !phase)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetProjectileRadius(), EndWidth);
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
                return StartCast
                       + (hero.NetworkPosition.Distance2D(StartPosition) - GetProjectileRadius(hero.NetworkPosition))
                       / GetProjectileSpeed() - Game.RawGameTime;
            }

            if (IsInPhase && hero.NetworkPosition.Distance2D(StartPosition) < StartWidth)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + CastPoint
                   + (hero.NetworkPosition.Distance2D(StartPosition) - GetProjectileRadius(hero.NetworkPosition))
                   / GetProjectileSpeed() - Game.RawGameTime;
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

        protected override float GetCastRange()
        {
            return Ability.GetRealCastRange() + 150;
        }

        protected override Vector3 GetProjectilePosition()
        {
            if (particleAdded)
            {
                return StartPosition.Extend(EndPosition, (Game.RawGameTime - StartCast) * GetProjectileSpeed());
            }

            return IsInPhase
                       ? StartPosition
                       : StartPosition.Extend(
                           EndPosition,
                           (Game.RawGameTime - StartCast - CastPoint) * GetProjectileSpeed());
        }

        #endregion
    }
}
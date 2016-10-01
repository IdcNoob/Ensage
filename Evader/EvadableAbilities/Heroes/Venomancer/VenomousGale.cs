namespace Evader.EvadableAbilities.Heroes
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class VenomousGale : LinearProjectile, IParticle
    {
        #region Fields

        private bool particleAdded;

        #endregion

        #region Constructors and Destructors

        public VenomousGale(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);

            Radius += 30;
        }

        #endregion

        #region Public Methods and Operators

        public void AddParticle(ParticleEffect particle)
        {
            if (Obstacle != null || !Owner.IsVisible)
            {
                return;
            }
            particleAdded = true;
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (particleAdded && (int)Owner.RotationDifference == 0)
            {
                StartCast = Game.RawGameTime;
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange());
                EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, Radius, Obstacle);
                particleAdded = false;
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), EndPosition);
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            return StartCast + (hero.NetworkPosition.Distance2D(StartPosition) - Radius) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        #endregion

        #region Methods

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 100;
        }

        protected override float GetProjectileSpeed()
        {
            return base.GetProjectileSpeed() + 100;
        }

        #endregion
    }
}
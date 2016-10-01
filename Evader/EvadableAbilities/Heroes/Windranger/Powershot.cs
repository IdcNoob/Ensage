namespace Evader.EvadableAbilities.Heroes
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using LinearProjectile = Base.LinearProjectile;

    internal class Powershot : LinearProjectile, IParticle
    {
        #region Fields

        private readonly float channelTime;

        private bool channelFix;

        private float channelingTime;

        private bool particleAdded;

        #endregion

        #region Constructors and Destructors

        public Powershot(Ability ability)
            : base(ability)
        {
            channelTime = ability.GetChannelTime(0);
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

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (particleAdded && (int)Owner.RotationDifference == 0)
            {
                StartCast = time;
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange());
                EndCast = StartCast + channelTime + GetCastRange() / GetProjectileSpeed();
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, Radius, Obstacle);
                particleAdded = false;
            }
            else if (Obstacle != null && !Ability.IsChanneling && !channelFix)
            {
                channelFix = true;
                channelingTime = time - StartCast;
                EndCast = time + GetCastRange() / GetProjectileSpeed();
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
            else if (Obstacle != null)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), EndPosition);
            }
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            channelFix = false;
            channelingTime = 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            return StartCast + channelTime - channelingTime
                   + (hero.Distance2D(StartPosition) - Radius) / GetProjectileSpeed() - Game.RawGameTime;
        }

        #endregion

        #region Methods

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return Ability.IsChanneling
                       ? StartPosition
                       : StartPosition.Extend(
                           EndPosition,
                           (Game.RawGameTime - StartCast - channelingTime) * GetProjectileSpeed());
        }

        #endregion
    }
}
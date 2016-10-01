namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;
    using Ensage.Common.Extensions;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class WildAxes : LinearProjectile
    {
        #region Fields

        private readonly float speed;

        #endregion

        #region Constructors and Destructors

        public WildAxes(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);

            speed = 800;
        }

        #endregion

        #region Public Methods and Operators

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
                EndPosition = Owner.InFront(GetCastRange() + Radius / 2);
                Obstacle = Pathfinder.AddObstacle(
                    // StartPosition.Extend(EndPosition, StartWidth),
                    StartPosition,
                    EndPosition,
                    Radius,
                    Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
        }

        #endregion

        #region Methods

        protected override float GetProjectileSpeed()
        {
            return speed;
        }

        #endregion
    }
}
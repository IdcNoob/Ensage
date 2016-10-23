namespace Evader.EvadableAbilities.Heroes.Slark
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Pounce : LinearProjectile, IParticle
    {
        #region Constructors and Destructors

        public Pounce(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
        }

        #endregion

        #region Public Methods and Operators

        public void AddParticle(ParticleEffect particle)
        {
            if (Obstacle != null || !AbilityOwner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            StartPosition = AbilityOwner.NetworkPosition;
            EndPosition = AbilityOwner.InFront(GetCastRange());
            EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
            Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        #endregion
    }
}
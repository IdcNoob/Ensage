namespace Evader.EvadableAbilities.Heroes
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Core.Abilities;

    using LinearProjectile = Base.LinearProjectile;

    internal class Pounce : LinearProjectile, IParticle
    {
        #region Constructors and Destructors

        public Pounce(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_slark_pounce_leash";

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
            if (Obstacle != null || !Owner.IsVisible)
            {
                return;
            }

            StartCast = Game.RawGameTime;
            StartPosition = Owner.NetworkPosition;
            EndPosition = Owner.InFront(GetCastRange());
            EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
            Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, Radius, Obstacle);
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (StartCast > 0 && time > EndCast)
            {
                End();
            }
        }

        #endregion
    }
}
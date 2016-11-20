namespace Evader.EvadableAbilities.Heroes.Clockwerk
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using SharpDX;

    using static Data.AbilityNames;

    internal class RocketFlare : AOE, IParticle
    {
        #region Fields

        private readonly float projectileSpeed;

        private Vector3 initialPosition;

        #endregion

        #region Constructors and Destructors

        public RocketFlare(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            projectileSpeed = ability.GetProjectileSpeed();
        }

        #endregion

        #region Public Methods and Operators

        public void AddParticle(ParticleEffectAddedEventArgs particleArgs)
        {
            if (particleArgs.Name.Contains("illumination"))
            {
                return;
            }

            StartCast = Game.RawGameTime;

            DelayAction.Add(
                1,
                () => {
                    StartPosition = particleArgs.ParticleEffect.GetControlPoint(1);
                    initialPosition = particleArgs.ParticleEffect.GetControlPoint(0);
                    EndCast = StartCast + StartPosition.Distance2D(initialPosition) / projectileSpeed;
                    Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
                });
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
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast - Game.RawGameTime;
        }

        #endregion
    }
}
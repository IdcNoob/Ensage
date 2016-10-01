namespace Evader.EvadableAbilities.Heroes
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Core.Abilities;

    internal sealed class PoisonNova : AOE, IParticle
    {
        #region Fields

        private readonly float speed;

        private readonly float tavelTime;

        private readonly float width;

        #endregion

        #region Constructors and Destructors

        public PoisonNova(Ability ability)
            : base(ability)
        {
            speed = ability.GetProjectileSpeed();
            tavelTime = GetRadius() / speed;
            width = 250;

            BlinkAbilities.AddRange(BlinkAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
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
            Position = Owner.NetworkPosition;
            EndCast = StartCast + tavelTime;
            Obstacle = Pathfinder.AddObstacle(Position, GetRadius(), Obstacle);
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

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            return StartCast + (hero.NetworkPosition.Distance2D(Position) - width) / speed - Game.RawGameTime;
        }

        #endregion
    }
}
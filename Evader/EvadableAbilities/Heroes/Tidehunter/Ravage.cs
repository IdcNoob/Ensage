namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Core.Abilities;

    internal sealed class Ravage : AOE
    {
        #region Fields

        private readonly float speed;

        private readonly float tavelTime;

        private readonly float width;

        #endregion

        #region Constructors and Destructors

        public Ravage(Ability ability)
            : base(ability)
        {
            speed = ability.GetProjectileSpeed() + 100;
            tavelTime = GetRadius() / speed;
            width = 350;

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (IsInPhase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                StartPosition = AbilityOwner.NetworkPosition;
                EndCast = StartCast + CastPoint + tavelTime;
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
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

            if (IsInPhase && hero.NetworkPosition.Distance2D(StartPosition) < width)
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + CastPoint + (hero.NetworkPosition.Distance2D(StartPosition) - width) / speed
                   - Game.RawGameTime;
        }

        #endregion
    }
}
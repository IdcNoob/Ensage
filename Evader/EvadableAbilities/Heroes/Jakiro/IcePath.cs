namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Core.Abilities;

    internal class IcePath : LinearAOE
    {
        #region Fields

        private readonly float additionalDelay;

        private readonly float[] duration = new float[4];

        #endregion

        #region Constructors and Destructors

        public IcePath(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);

            additionalDelay = ability.AbilitySpecialData.First(x => x.Name == "path_delay").Value;

            for (var i = 0u; i < duration.Length; i++)
            {
                duration[i] = ability.AbilitySpecialData.First(x => x.Name == "duration").GetValue(i);
            }

            ObstacleStays = true;
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
                EndCast = StartCast + CastPoint + additionalDelay + GetDuration();
            }
            else if (phase && Obstacle == null && (int)Owner.RotationDifference == 0)
            {
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && time > EndCast)
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return StartCast + CastPoint + additionalDelay - Game.RawGameTime;
        }

        #endregion

        #region Methods

        private float GetDuration()
        {
            return duration[Ability.Level - 1];
        }

        #endregion
    }
}
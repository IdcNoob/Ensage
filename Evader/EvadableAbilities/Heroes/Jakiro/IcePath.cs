namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using Utils;

    using static Core.Abilities;

    internal class IcePath : LinearAOE
    {
        #region Fields

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
            CounterAbilities.Add(SnowBall);

            CounterAbilities.Remove("bane_nightmare");

            AdditionalDelay = ability.AbilitySpecialData.First(x => x.Name == "path_delay").Value;

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
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay + GetDuration();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.InFront(-GetRadius() * 0.9f);
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 5);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
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
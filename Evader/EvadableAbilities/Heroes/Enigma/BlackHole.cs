namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using SharpDX;

    using static Core.Abilities;

    internal class BlackHole : LinearAOE
    {
        #region Fields

        private readonly float channelTime;

        private Vector3 position;

        #endregion

        #region Constructors and Destructors

        public BlackHole(Ability ability)
            : base(ability)
        {
            channelTime = ability.GetChannelTime(0);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(StrongDefUltimates);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Manta);

            //todo fix
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (IsInPhase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                position = Owner.NetworkPosition;
                EndCast = StartCast + CastPoint;
                Obstacle = Pathfinder.AddObstacle(position, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && time > EndCast && !IgnoreRemainingTime())
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast + channelTime - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(float remainingTime = 0)
        {
            return Ability.IsChanneling;
        }

        #endregion
    }
}
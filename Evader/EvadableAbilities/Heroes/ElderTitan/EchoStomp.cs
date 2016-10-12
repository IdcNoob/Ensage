namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class EchoStomp : AOE
    {
        #region Fields

        private readonly float channelTime;

        #endregion

        #region Constructors and Destructors

        public EchoStomp(Ability ability)
            : base(ability)
        {
            //todo add astral spirit
            channelTime = ability.GetChannelTime(0);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                StartPosition = AbilityOwner.NetworkPosition;
                EndCast = StartCast + CastPoint;
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast && !Ability.IsChanneling)
            {
                End();
            }
        }

        public override float GetRemainingDisableTime()
        {
            return GetRemainingTime() - 0.05f;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast + channelTime - Game.RawGameTime;
        }

        #endregion
    }
}
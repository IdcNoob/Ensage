namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using static Core.Abilities;

    internal class FreezingField : AOE
    {
        #region Fields

        private readonly float channelTime;

        #endregion

        #region Constructors and Destructors

        public FreezingField(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);

            IgnorePathfinder = true;
            channelTime = ability.GetChannelTime(0);
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            var time = Game.RawGameTime;

            if (IsInPhase && StartCast + CastPoint <= time)
            {
                StartCast = time;
                Position = Owner.NetworkPosition;
                EndCast = StartCast + CastPoint;
                Obstacle = Pathfinder.AddObstacle(Position, GetRadius(), Obstacle);
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
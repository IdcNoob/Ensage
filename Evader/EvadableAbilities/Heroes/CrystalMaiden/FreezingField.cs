namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    using UsableAbilities.Base;

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
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                StartPosition = AbilityOwner.NetworkPosition;
                EndCast = StartCast + CastPoint;
                Obstacle = Pathfinder.AddObstacle(StartPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast && !IgnoreRemainingTime(null))
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast + channelTime - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return Ability.IsChanneling;
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes
{
    using System.Linq;

    using Base;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Core.Abilities;

    internal class FingerOfDeath : LinearTarget
    {
        #region Fields

        private readonly float additionalDelay;

        #endregion

        #region Constructors and Destructors

        public FingerOfDeath(Ability ability)
            : base(ability)
        {
            //todo add aghanim

            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Add(NetherWard);
            CounterAbilities.AddRange(Invis);

            additionalDelay = Ability.AbilitySpecialData.First(x => x.Name == "damage_delay").Value;
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
                EndCast = StartCast + CastPoint + additionalDelay;
            }
            else if (phase && Obstacle == null && (int)Owner.RotationDifference == 0)
            {
                StartPosition = Owner.NetworkPosition;
                EndPosition = Owner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetWidth(), Obstacle);
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
    }
}
namespace Evader.EvadableAbilities.Heroes.Bane
{
    using System.Linq;

    using Base;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    internal class FiendsGrip : LinearTarget
    {
        #region Fields

        private readonly float aghanimDuration;

        private readonly float duration;

        #endregion

        #region Constructors and Destructors

        public FiendsGrip(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(NetherWard);

            duration = Ability.AbilitySpecialData.First(x => x.Name == "fiend_grip_duration").Value;
            aghanimDuration = Ability.AbilitySpecialData.First(x => x.Name == "fiend_grip_duration_scepter").Value;
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + GetDuration();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + 150);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast && !IgnoreRemainingTime(null))
            {
                End();
            }
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast + GetDuration() - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return Ability.IsChanneling;
        }

        #endregion

        #region Methods

        private float GetDuration()
        {
            return AbilityOwner.AghanimState() ? aghanimDuration : duration;
        }

        #endregion
    }
}
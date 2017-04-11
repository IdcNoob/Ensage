namespace Evader.EvadableAbilities.Heroes.WitchDoctor
{
    using Base;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using UsableAbilities.Base;

    using static Data.AbilityNames;

    internal class DeathWard : LinearAOE
    {
        private readonly float duration;

        public DeathWard(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);

            duration = Ability.GetChannelTime(0);
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay;
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.InFront(-GetRadius() * 0.9f);
                EndPosition = AbilityOwner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast && !IgnoreRemainingTime(null))
            {
                End();
            }
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return EndCast + duration - Game.RawGameTime;
        }

        public override bool IgnoreRemainingTime(UsableAbility ability, float remainingTime = 0)
        {
            return Ability.IsChanneling;
        }
    }
}
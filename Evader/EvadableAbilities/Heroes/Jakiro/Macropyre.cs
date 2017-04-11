namespace Evader.EvadableAbilities.Heroes.Jakiro
{
    using System.Linq;

    using Base;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class Macropyre : LinearAOE
    {
        private readonly float duration;

        public Macropyre(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;
            ObstacleStays = true;

            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(AllyShields);

            duration = ability.AbilitySpecialData.First(x => x.Name == "duration").Value;
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay + duration;
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.InFront(-100);
                EndPosition = AbilityOwner.InFront(GetCastRange() + 100);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }
    }
}
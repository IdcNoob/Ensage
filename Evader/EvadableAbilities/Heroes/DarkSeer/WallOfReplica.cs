namespace Evader.EvadableAbilities.Heroes.DarkSeer
{
    using Base;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class WallOfReplica : LinearAOE
    {
        public WallOfReplica(Ability ability)
            : base(ability)
        {
            DisablePathfinder = true;

            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
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
                StartPosition = AbilityOwner.Position;
                EndPosition = AbilityOwner.InFront(GetCastRange());
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), AbilityOwner.Position);
            AbilityDrawer.DrawRectangle(StartPosition, EndPosition, GetRadius());
        }

        protected override float GetRadius()
        {
            return base.GetRadius() - 400;
        }
    }
}
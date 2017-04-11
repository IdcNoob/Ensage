namespace Evader.EvadableAbilities.Heroes.Invoker
{
    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class DeafeningBlast : LinearProjectile
    {
        private readonly Ability talent;

        public DeafeningBlast(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            talent = AbilityOwner.FindSpell("special_bonus_unique_invoker_2");
        }

        public override bool CanBeStopped()
        {
            return false;
        }

        public override void Check()
        {
            if (StartCast > 0 && Obstacle != null && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (StartCast <= 0 && Obstacle == null && (Ability.IsInAbilityPhase || TimeSinceCast() < 0.1)
                     && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay + GetCastRange() / GetProjectileSpeed();
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 2);
                Obstacle = IsAOE()
                               ? Pathfinder.AddObstacle(StartPosition, StartPosition.Distance2D(EndPosition), Obstacle)
                               : Pathfinder.AddObstacle(
                                   StartPosition,
                                   EndPosition,
                                   GetRadius(),
                                   GetEndRadius(),
                                   Obstacle);
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);

            if (IsAOE())
            {
                AbilityDrawer.DrawCircle(StartPosition, StartPosition.Distance2D(EndPosition));
            }
            else
            {
                AbilityDrawer.DrawArcRectangle(StartPosition, EndPosition, GetRadius(), GetEndRadius());
                AbilityDrawer.DrawCircle(StartPosition, (GetRadius() + GetEndRadius()) / 2);
                AbilityDrawer.UpdateCirclePosition(GetProjectilePosition());
            }
        }

        private bool IsAOE()
        {
            return talent?.Level > 0;
        }
    }
}
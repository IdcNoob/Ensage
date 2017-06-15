namespace Evader.EvadableAbilities.Heroes.Invoker
{
    using System;

    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class Tornado : LinearProjectile, IUnit
    {
        private readonly Ability wex;

        private Unit abilityUnit;

        public Tornado(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(HurricanePike);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);

            wex = AbilityOwner.FindSpell("invoker_wex");
        }

        public void AddUnit(Unit unit)
        {
            abilityUnit = unit;
            StartCast = Game.RawGameTime;
            EndCast = Game.RawGameTime + GetCastRange() / GetProjectileSpeed();
            StartPosition = unit.Position;
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
            else if (Obstacle == null && IsAbilityUnitValid() && abilityUnit.IsVisible)
            {
                var unitPosition = abilityUnit.Position;
                if (unitPosition == StartPosition)
                {
                    return;
                }

                EndPosition = StartPosition.Extend(abilityUnit.Position, GetCastRange());

                if (Math.Abs(EndPosition.Distance2D(StartPosition) - GetCastRange()) > 50)
                {
                    return;
                }

                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), Obstacle);
            }
            else if (Obstacle != null)
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius());
            }
        }

        public override void Draw()
        {
            if (Obstacle == null)
            {
                return;
            }

            AbilityDrawer.DrawTime(GetRemainingTime(), StartPosition);
            AbilityDrawer.DrawArcRectangle(StartPosition, EndPosition, GetRadius(), GetEndRadius());
            AbilityDrawer.DrawCircle(StartPosition, (GetRadius() + GetEndRadius()) / 2);

            AbilityDrawer.UpdateCirclePosition(GetProjectilePosition());
        }

        public override void End()
        {
            base.End();
            abilityUnit = null;
        }

        public override float GetRemainingDisableTime()
        {
            return 0;
        }

        protected override float GetCastRange()
        {
            return (400 + (wex?.Level ?? 8) * 400) * 1.2f;
        }

        private bool IsAbilityUnitValid()
        {
            return abilityUnit != null && abilityUnit.IsValid;
        }
    }
}
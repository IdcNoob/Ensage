namespace Evader.EvadableAbilities.Heroes.ShadowDemon
{
    using Base.Interfaces;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class ShadowPoison : LinearProjectile, IUnit
    {
        private Unit abilityUnit;

        private bool fowCast;

        public ShadowPoison(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
        }

        public void AddUnit(Unit unit)
        {
            if (AbilityOwner.IsVisible)
            {
                return;
            }

            abilityUnit = unit;
            StartCast = Game.RawGameTime;
            EndCast = StartCast + GetCastRange() / GetProjectileSpeed();
            StartPosition = unit.Position.SetZ(350);
            fowCast = true;
        }

        public override bool CanBeStopped()
        {
            return !fowCast && base.CanBeStopped();
        }

        public override void Check()
        {
            if (fowCast && Obstacle == null)
            {
                if (!IsAbilityUnitValid() || !abilityUnit.IsVisible)
                {
                    return;
                }

                var position = StartPosition.Extend(abilityUnit.Position.SetZ(350), GetCastRange());

                if (position.Distance2D(StartPosition) < 50)
                {
                    return;
                }

                EndPosition = position;
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), GetEndRadius(), Obstacle);
            }
            else if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + AdditionalDelay + GetCastRange() / GetProjectileSpeed();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = AbilityOwner.InFront(GetCastRange() + GetRadius() / 2);
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), GetEndRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !CanBeStopped())
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius(), GetEndRadius());
            }
        }

        public override void End()
        {
            if (Obstacle == null)
            {
                return;
            }

            base.End();
            fowCast = false;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.NetworkPosition;

            if (IsInPhase && AbilityOwner.IsVisible && position.Distance2D(StartPosition) < GetRadius())
            {
                return StartCast + CastPoint - Game.RawGameTime;
            }

            return StartCast + (fowCast ? 0 : CastPoint)
                   + (position.Distance2D(StartPosition) - GetRadius()) / GetProjectileSpeed() - Game.RawGameTime;
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return base.GetProjectilePosition(fowCast);
        }

        private bool IsAbilityUnitValid()
        {
            return abilityUnit != null && abilityUnit.IsValid;
        }
    }
}
namespace Evader.EvadableAbilities.Base
{
    using System.Linq;

    using Common;

    using Ensage;
    using Ensage.Common.AbilityInfo;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    internal abstract class LinearProjectile : LinearAOE
    {
        private readonly float endRadius;

        private readonly float speed;

        private readonly float startRadius;

        protected LinearProjectile(Ability ability)
            : base(ability)
        {
            speed = ability.GetProjectileSpeed();

            var widthString = AbilityDatabase.Find(Name).Width;
            var endWidthString = AbilityDatabase.Find(Name).EndWidth;

            if (string.IsNullOrEmpty(widthString))
            {
                var radius = ability.GetRadius();

                if ((int)ability.GetCastRange() != (int)radius)
                {
                    startRadius = endRadius = radius + 60;
                }
                else
                {
                    startRadius = endRadius = 250;
                }
            }
            else
            {
                startRadius = ability.AbilitySpecialData.First(x => x.Name == widthString).Value + 60;
                endRadius = ability.AbilitySpecialData.FirstOrDefault(x => x.Name == endWidthString)?.Value + 60
                            ?? startRadius;
            }

            if (startRadius > endRadius)
            {
                endRadius = startRadius;
            }

            Debugger.WriteLine("// Speed: " + speed);
            Debugger.WriteLine("// StartRadius: " + startRadius);
            Debugger.WriteLine("// EndRadius: " + endRadius);
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
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

        public float GetProjectileRadius(Vector3 position = new Vector3())
        {
            if (GetRadius().Equals(GetEndRadius()))
            {
                return GetRadius();
            }

            if (position.IsZero)
            {
                position = GetProjectilePosition();
            }

            return (GetEndRadius() - GetRadius()) * (StartPosition.Distance2D(position) / GetCastRange()) + GetRadius();
        }

        public virtual float GetProjectileSpeed()
        {
            return speed;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.NetworkPosition;

            if (IsInPhase && position.Distance2D(StartPosition) <= GetRadius())
            {
                return StartCast + CastPoint + AdditionalDelay - Game.RawGameTime;
            }

            return StartCast + CastPoint + AdditionalDelay
                   + (position.Distance2D(StartPosition) - GetProjectileRadius(position) - 60) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        protected virtual float GetEndRadius()
        {
            return endRadius;
        }

        protected virtual Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return IsInPhase
                       ? StartPosition
                       : StartPosition.Extend(
                           EndPosition,
                           (Game.RawGameTime - StartCast - (ignoreCastPoint ? 0 : CastPoint)) * GetProjectileSpeed());
        }

        protected override float GetRadius()
        {
            return startRadius;
        }
    }
}
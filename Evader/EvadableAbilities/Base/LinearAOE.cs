namespace Evader.EvadableAbilities.Base
{
    using Common;

    using Ensage;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal abstract class LinearAOE : AOE
    {
        protected LinearAOE(Ability ability)
            : base(ability)
        {
            Debugger.WriteLine("// Cast range: " + Ability.GetRealCastRange());
        }

        protected Vector3 EndPosition { get; set; }

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
            AbilityDrawer.DrawDoubleArcRectangle(StartPosition, EndPosition, GetRadius());
        }

        protected virtual float GetCastRange()
        {
            return Ability.GetRealCastRange();
        }
    }
}
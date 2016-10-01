namespace Evader.Utils
{
    using System;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    internal static class Utils
    {
        #region Public Methods and Operators

        public static void DrawRectangle(
            Vector3 startPosition,
            Vector3 endPosition,
            float startWidth,
            float endWidth = 0)
        {
            if (endWidth <= 0)
            {
                endWidth = startWidth;
            }

            endPosition = startPosition.Extend(endPosition, startPosition.Distance2D(endPosition) + endWidth / 2);

            var difference = startPosition - endPosition;
            var rotation = difference.Rotated(MathUtil.DegreesToRadians(90));
            rotation.Normalize();

            var start = rotation * startWidth;
            var end = rotation * endWidth;

            var rightStartPosition = startPosition + start;
            var leftStartPosition = startPosition - start;
            var rightEndPosition = endPosition + end;
            var leftEndPosition = endPosition - end;

            Vector2 leftStart, rightStart, leftEnd, rightEnd;
            Drawing.WorldToScreen(leftStartPosition, out leftStart);
            Drawing.WorldToScreen(rightStartPosition, out rightStart);
            Drawing.WorldToScreen(leftEndPosition, out leftEnd);
            Drawing.WorldToScreen(rightEndPosition, out rightEnd);

            Drawing.DrawLine(leftStart, rightStart, Color.Orange);
            Drawing.DrawLine(rightStart, rightEnd, Color.Orange);
            Drawing.DrawLine(rightEnd, leftEnd, Color.Orange);
            Drawing.DrawLine(leftEnd, leftStart, Color.Orange);
        }

        //temp
        public static double FindRelativeAngle(this Unit unit, Vector3 pos)
        {
            var angle = Math.Atan2(pos.Y - unit.Position.Y, pos.X - unit.Position.X) - unit.RotationRad;

            if (Math.Abs(angle) > Math.PI)
            {
                angle = Math.PI * 2 - Math.Abs(angle);
            }

            return Math.Abs(angle);
        }

        public static float GetRealCastRange(this Ability ability)
        {
            var castRange = ability.GetCastRange();

            if (!ability.IsAbilityBehavior(AbilityBehavior.NoTarget))
            {
                castRange += Math.Max(castRange / 9, 80);
            }
            else
            {
                castRange += Math.Max(castRange / 7, 40);
            }

            return castRange;
        }

        //temp
        public static double GetTurnTime(this Unit unit, Vector3 position)
        {
            if (unit.ClassID == ClassID.CDOTA_Unit_Hero_Wisp)
            {
                return 0;
            }

            var angle = unit.FindRelativeAngle(position);

            if (angle <= 0.2)
            {
                return 0;
            }

            var turnRate = unit.GetTurnRate();

            if (unit.HasModifier("modifier_medusa_stone_gaze_slow"))
            {
                turnRate *= 0.65;
            }
            if (unit.HasModifier("modifier_batrider_sticky_napalm"))
            {
                turnRate *= 0.3;
            }

            return 0.03 / turnRate * angle;
        }

        //temp
        public static double GetTurnTime(this Unit unit, Unit target)
        {
            return unit.GetTurnTime(target.Position);
        }

        #endregion
    }
}
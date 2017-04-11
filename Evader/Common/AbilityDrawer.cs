namespace Evader.Common
{
    using Core;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    internal class AbilityDrawer
    {
        public enum Type
        {
            Any,

            Circle,

            Rectangle
        }

        private readonly ParticleEffect[] rectangle = new ParticleEffect[4];

        private ParticleEffect circle;

        public AbilityDrawer()
        {
            TextColor = Color.White;
            TextSize = new Vector2(20);
            ParticleColor = new Vector3(255, 100, 50);
        }

        public Vector3 ParticleColor { get; set; }

        public Color TextColor { get; set; }

        public Vector2 TextSize { get; set; }

        public void Dispose(Type figure = Type.Any)
        {
            switch (figure)
            {
                case Type.Any:
                    DisposeCircle();
                    DisposeRectangle();
                    break;
                case Type.Circle:
                    DisposeCircle();
                    break;
                case Type.Rectangle:
                    DisposeRectangle();
                    break;
            }
        }

        public void DrawArcRectangle(Vector3 startPosition, Vector3 endPosition, float startRadius, float endRadius = 0)
        {
            if (rectangle[0] != null)
            {
                return;
            }

            if (endRadius <= 0)
            {
                endRadius = startRadius;
            }

            var difference = startPosition - endPosition;
            var rotation = difference.Rotated(MathUtil.DegreesToRadians(90));
            rotation.Normalize();

            var start = rotation * startRadius;
            var end = rotation * endRadius;

            var correctedEnd = startPosition.Extend(
                endPosition,
                startPosition.Distance2D(endPosition) - endRadius * 0.45f);

            var rightStartPosition = startPosition + start;
            var leftStartPosition = startPosition - start;
            var rightEndPosition = correctedEnd + end;
            var leftEndPosition = correctedEnd - end;

            rectangle[0] = DrawLine(rightStartPosition, rightEndPosition);
            rectangle[1] = DrawLine(rightStartPosition, leftStartPosition);
            rectangle[2] = DrawLine(leftStartPosition, leftEndPosition);
            rectangle[3] = DrawArc(endPosition, startPosition, endRadius);
        }

        public void DrawCircle(Vector3 position, float radius)
        {
            if (!Variables.Menu.Debug.DrawAbilities || circle != null)
            {
                return;
            }

            circle = new ParticleEffect(@"materials\ensage_ui\particles\drag_selected_ring_mod.vpcf", position);
            circle.SetControlPoint(1, ParticleColor);
            circle.SetControlPoint(2, new Vector3(radius * -1, 255, 0));
        }

        public void DrawDoubleArcRectangle(
            Vector3 startPosition,
            Vector3 endPosition,
            float startRadius,
            float endRadius = 0)
        {
            if (rectangle[0] != null)
            {
                return;
            }

            if (endRadius <= 0)
            {
                endRadius = startRadius;
            }

            var difference = startPosition - endPosition;
            var rotation = difference.Rotated(MathUtil.DegreesToRadians(90));
            rotation.Normalize();

            var start = rotation * startRadius;
            var end = rotation * endRadius;

            var correctedEnd = startPosition.Extend(
                endPosition,
                startPosition.Distance2D(endPosition) - endRadius * 0.45f);

            var correctedStart = startPosition.Extend(endPosition, startRadius);

            var rightStartPosition = correctedStart + start;
            var leftStartPosition = correctedStart - start;
            var rightEndPosition = correctedEnd + end;
            var leftEndPosition = correctedEnd - end;

            rectangle[0] = DrawLine(rightStartPosition, rightEndPosition);
            rectangle[1] = DrawArc(startPosition.Extend(endPosition, startRadius * 0.55f), endPosition, startRadius);
            rectangle[2] = DrawLine(leftStartPosition, leftEndPosition);
            rectangle[3] = DrawArc(endPosition, startPosition, endRadius);
        }

        public void DrawRectangle(Vector3 startPosition, Vector3 endPosition, float startWidth, float endWidth = 0)
        {
            if (rectangle[0] != null)
            {
                return;
            }

            if (endWidth <= 0)
            {
                endWidth = startWidth;
            }

            var difference = startPosition - endPosition;
            var rotation = difference.Rotated(MathUtil.DegreesToRadians(90));
            rotation.Normalize();

            var start = rotation * startWidth;
            var end = rotation * endWidth;

            var rightStartPosition = startPosition + start;
            var leftStartPosition = startPosition - start;
            var rightEndPosition = endPosition + end;
            var leftEndPosition = endPosition - end;

            rectangle[0] = DrawLine(rightStartPosition, rightEndPosition);
            rectangle[1] = DrawLine(rightStartPosition, leftStartPosition);
            rectangle[2] = DrawLine(leftStartPosition, leftEndPosition);
            rectangle[3] = DrawLine(leftEndPosition, rightEndPosition);
        }

        public void DrawTime(float time, Vector3 position)
        {
            Drawing.DrawText(
                time.ToString("0.00"),
                "Arial",
                Drawing.WorldToScreen(position),
                TextSize,
                TextColor,
                FontFlags.None);
        }

        public void UpdateCirclePosition(Vector3 position)
        {
            circle?.SetControlPoint(0, position);
        }

        public void UpdateRectanglePosition(
            Vector3 startPosition,
            Vector3 endPosition,
            float startWidth,
            float endWidth = 0)
        {
            if (rectangle[0] == null)
            {
                return;
            }

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

            rectangle[0].SetControlPoint(1, rightStartPosition);
            rectangle[0].SetControlPoint(2, rightEndPosition);

            rectangle[1].SetControlPoint(1, rightStartPosition);
            rectangle[1].SetControlPoint(2, leftStartPosition);

            rectangle[2].SetControlPoint(1, leftStartPosition);
            rectangle[2].SetControlPoint(2, leftEndPosition);

            rectangle[3].SetControlPoint(1, leftEndPosition);
            rectangle[3].SetControlPoint(2, rightEndPosition);
        }

        private void DisposeCircle()
        {
            circle?.Dispose();
            circle = null;
        }

        private void DisposeRectangle()
        {
            for (var i = 0; i < rectangle.Length; i++)
            {
                rectangle[i]?.Dispose();
                rectangle[i] = null;
            }
        }

        private ParticleEffect DrawArc(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            var arc = new ParticleEffect(@"materials\ensage_ui\particles\semicircle_v2.vpcf", startPosition);
            arc.SetControlPoint(1, endPosition);
            arc.SetControlPoint(2, new Vector3(radius * 1.12f, 0, 0));
            arc.SetControlPoint(3, ParticleColor);
            arc.SetControlPoint(4, new Vector3(255, 15, 0));

            return arc;
        }

        private ParticleEffect DrawLine(Vector3 startPosition, Vector3 endPosition)
        {
            var line = new ParticleEffect(@"materials\ensage_ui\particles\line.vpcf", startPosition);
            line.SetControlPoint(2, endPosition);
            line.SetControlPoint(3, new Vector3(255, 15, 0));
            line.SetControlPoint(4, ParticleColor);

            return line;
        }
    }
}
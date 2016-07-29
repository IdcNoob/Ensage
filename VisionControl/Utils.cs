namespace VisionControl
{
    using System;

    using Ensage;

    using SharpDX;

    internal static class Utils
    {
        #region Public Methods and Operators

        public static Vector2 WorldToMiniMap(Vector3 pos, Vector2 size)
        {
            const float MapLeft = -8000;
            const float MapTop = 7350;
            const float MapRight = 7500;
            const float MapBottom = -7200;
            var mapWidth = Math.Abs(MapLeft - MapRight);
            var mapHeight = Math.Abs(MapBottom - MapTop);

            var x = pos.X - MapLeft;
            var y = pos.Y - MapBottom;

            float dx, dy, px, py;
            if (Math.Round((float)Drawing.Width / Drawing.Height, 1) >= 1.7)
            {
                dx = 272f / 1920f * Drawing.Width;
                dy = 261f / 1080f * Drawing.Height;
                px = 11f / 1920f * Drawing.Width;
                py = 11f / 1080f * Drawing.Height;
            }
            else if (Math.Round((float)Drawing.Width / Drawing.Height, 1) >= 1.5)
            {
                dx = 267f / 1680f * Drawing.Width;
                dy = 252f / 1050f * Drawing.Height;
                px = 10f / 1680f * Drawing.Width;
                py = 11f / 1050f * Drawing.Height;
            }
            else
            {
                dx = 255f / 1280f * Drawing.Width;
                dy = 229f / 1024f * Drawing.Height;
                px = 6f / 1280f * Drawing.Width;
                py = 9f / 1024f * Drawing.Height;
            }
            var minimapMapScaleX = dx / mapWidth;
            var minimapMapScaleY = dy / mapHeight;

            var scaledX = Math.Min(Math.Max(x * minimapMapScaleX, 0), dx);
            var scaledY = Math.Min(Math.Max(y * minimapMapScaleY, 0), dy);

            var screenX = px + scaledX;
            var screenY = Drawing.Height - scaledY - py;

            return new Vector2((float)Math.Floor(screenX - size.X / 2), (float)Math.Floor(screenY - size.Y / 2));
        }

        #endregion
    }
}
namespace Evader.Common
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;

    using SharpDX;

    internal static class MouseEmulation
    {
        #region Constants

        private const int MouseSpeed = 30;

        #endregion

        #region Static Fields

        private static readonly Random Random = new Random();

        #endregion

        #region Public Methods and Operators

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point p);

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public static void Move(int x, int y, int rx, int ry)
        {
            Point c;
            GetCursorPos(out c);

            x += Random.Next(rx);
            y += Random.Next(ry);

            var randomSpeed = Math.Max((Random.Next(MouseSpeed) / 2.0 + MouseSpeed) / 10.0, 0.1);

            WindMouse(
                c.X,
                c.Y,
                x,
                y,
                9.0,
                3.0,
                10.0 / randomSpeed,
                15.0 / randomSpeed,
                10.0 * randomSpeed,
                10.0 * randomSpeed);
        }

        #endregion

        #region Methods

        private static double Hypot(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        private static void WindMouse(
            double xs,
            double ys,
            double xe,
            double ye,
            double gravity,
            double wind,
            double minWait,
            double maxWait,
            double maxStep,
            double targetArea)
        {
            double windX = 0, windY = 0, veloX = 0, veloY = 0;
            int newX = (int)Math.Round(xs), newY = (int)Math.Round(ys);

            var waitDiff = maxWait - minWait;
            var sqrt2 = Math.Sqrt(2.0);
            var sqrt3 = Math.Sqrt(3.0);
            var sqrt5 = Math.Sqrt(5.0);

            var dist = Hypot(xe - xs, ye - ys);

            while (dist > 1.0)
            {
                wind = Math.Min(wind, dist);

                if (dist >= targetArea)
                {
                    var w = Random.Next((int)Math.Round(wind) * 2 + 1);
                    windX = windX / sqrt3 + (w - wind) / sqrt5;
                    windY = windY / sqrt3 + (w - wind) / sqrt5;
                }
                else
                {
                    windX = windX / sqrt2;
                    windY = windY / sqrt2;
                    if (maxStep < 3)
                    {
                        maxStep = Random.Next(3) + 3.0;
                    }
                    else
                    {
                        maxStep = maxStep / sqrt5;
                    }
                }

                veloX += windX;
                veloY += windY;
                veloX = veloX + gravity * (xe - xs) / dist;
                veloY = veloY + gravity * (ye - ys) / dist;

                if (Hypot(veloX, veloY) > maxStep)
                {
                    var randomDist = maxStep / 2.0 + Random.Next((int)Math.Round(maxStep) / 2);
                    var veloMag = Hypot(veloX, veloY);
                    veloX = (veloX / veloMag) * randomDist;
                    veloY = (veloY / veloMag) * randomDist;
                }

                var oldX = (int)Math.Round(xs);
                var oldY = (int)Math.Round(ys);
                xs += veloX;
                ys += veloY;
                dist = Hypot(xe - xs, ye - ys);
                newX = (int)Math.Round(xs);
                newY = (int)Math.Round(ys);

                if (oldX != newX || oldY != newY)
                {
                    SetCursorPos(newX, newY);
                }

                var step = Hypot(xs - oldX, ys - oldY);
                var wait = (int)Math.Round(waitDiff * (step / maxStep) + minWait);
                Thread.Sleep(wait);
            }

            var endX = (int)Math.Round(xe);
            var endY = (int)Math.Round(ye);
            if (endX != newX || endY != newY)
            {
                SetCursorPos(endX, endY);
            }
        }

        #endregion
    }
}
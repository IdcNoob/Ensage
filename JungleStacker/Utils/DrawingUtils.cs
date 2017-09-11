namespace JungleStacker.Utils
{
    using System;

    using Ensage;

    using SharpDX;

    internal static class DrawingUtils
    {
        public static void RoundedRectangle(float x, float y, float w, float h, int iSmooth, Color color)
        {
            var pt = new Vector2[4];

            // Get all corners 
            pt[0].X = x + (w - iSmooth);
            pt[0].Y = y + (h - iSmooth);

            pt[1].X = x + iSmooth;
            pt[1].Y = y + (h - iSmooth);

            pt[2].X = x + iSmooth;
            pt[2].Y = y + iSmooth;

            pt[3].X = x + w - iSmooth;
            pt[3].Y = y + iSmooth;

            // Draw cross 
            Drawing.DrawRect(new Vector2(x, y + iSmooth), new Vector2(w, h - iSmooth * 2), color);

            Drawing.DrawRect(new Vector2(x + iSmooth, y), new Vector2(w - iSmooth * 2, h), color);

            float fDegree = 0;

            for (var i = 0; i < 4; i++)
            {
                for (var k = fDegree; k < fDegree + Math.PI * 2 / 4f; k += (float)(1 * (Math.PI / 180.0f)))
                {
                    // Draw quarter circles on every corner
                    Drawing.DrawLine(
                        new Vector2(pt[i].X, pt[i].Y),
                        new Vector2(pt[i].X + (float)(Math.Cos(k) * iSmooth), pt[i].Y + (float)(Math.Sin(k) * iSmooth)),
                        color);
                }

                fDegree += (float)(Math.PI * 2) / 4; // quarter circle offset 
            }
        }
    }
}
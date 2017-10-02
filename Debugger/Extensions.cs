namespace Debugger
{
    using System;

    using Ensage.SDK.Menu;

    using SharpDX;

    internal static class Extensions
    {
        public static void AddAsterisk(this MenuFactory menu)
        {
            if (!menu.Target.DisplayName.EndsWith("*"))
            {
                menu.Target.DisplayName += "*";
            }
        }

        public static void RemoveAsterisk(this MenuFactory menu)
        {
            if (menu.Target.DisplayName.EndsWith("*"))
            {
                menu.Target.DisplayName = menu.Target.DisplayName.TrimEnd('*');
            }
        }

        public static Color SetAlpha(this Color color, int alpha)
        {
            return new Color(color.R, color.G, color.B, alpha);
        }

        public static string ToCopyFormat(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            if (obj is Enum)
            {
                return obj.GetType().Name + "." + obj;
            }

            if (obj is Vector3)
            {
                var v3 = (Vector3)obj;
                return (int)v3.X + ", " + (int)v3.Y + ", " + (int)v3.Z;
            }

            return obj.ToString();
        }
    }
}
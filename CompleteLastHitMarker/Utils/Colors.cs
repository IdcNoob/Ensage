namespace CompleteLastHitMarker.Utils
{
    using SharpDX;

    internal static class Colors
    {
        public static Color SetBlue(this Color color, int value)
        {
            return new Color(color.R, color.G, value);
        }

        public static Color SetGreen(this Color color, int value)
        {
            return new Color(color.R, value, color.B);
        }

        public static Color SetRed(this Color color, int value)
        {
            return new Color(value, color.G, color.B);
        }
    }
}
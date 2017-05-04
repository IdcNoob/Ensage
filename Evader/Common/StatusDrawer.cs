namespace Evader.Common
{
    using Core;
    using Core.Menus;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class StatusDrawer
    {
        private const string BkbText = "BKB";

        private const string EvaderText = "Evader";

        private const string Font = "Arial";

        private const string MovementBlockedText = "Movement Blocked";

        private const string PathfinderText = "Pathfinder";

        private readonly Vector2 bkbTextPosition;

        private readonly Vector2 evaderTextPosition;

        private readonly Vector2 movementBlockedPosition;

        private readonly Vector2 pathfinderTextPosition;

        private readonly Color textColor;

        private readonly Vector2 textSize;

        public StatusDrawer()
        {
            textSize = new Vector2(21);
            textColor = Color.Orange;

            var x = HUDInfo.ScreenSizeX();
            var evaderTextSize = Drawing.MeasureText(EvaderText, Font, textSize, FontFlags.None);
            var pathfinderTextSize = Drawing.MeasureText(PathfinderText, Font, textSize, FontFlags.None);
            var bkbTextSize = Drawing.MeasureText(BkbText, Font, textSize, FontFlags.None);
            var movementBlockedText = Drawing.MeasureText(MovementBlockedText, Font, textSize, FontFlags.None);

            evaderTextPosition = new Vector2(x - evaderTextSize.X - 10, evaderTextSize.Y + 25);

            pathfinderTextPosition = new Vector2(
                x - pathfinderTextSize.X - 10,
                pathfinderTextSize.Y + evaderTextPosition.Y);

            bkbTextPosition = new Vector2(x - bkbTextSize.X - 10, bkbTextSize.Y + pathfinderTextPosition.Y);

            movementBlockedPosition = new Vector2(
                x - movementBlockedText.X - 10,
                movementBlockedText.Y + bkbTextPosition.Y);
        }

        private static HotkeysMenu Menu => Variables.Menu.Hotkeys;

        private MultiSleeper sleeper => Variables.Sleeper;

        public void Draw()
        {
            if (!Menu.EnabledEvader)
            {
                return;
            }

            Drawing.DrawText(EvaderText, Font, evaderTextPosition, textSize, textColor, FontFlags.None);

            if (Menu.PathfinderMode != Pathfinder.EvadeMode.None)
            {
                Drawing.DrawText(
                    PathfinderText,
                    Font,
                    pathfinderTextPosition,
                    textSize,
                    Menu.PathfinderMode == Pathfinder.EvadeMode.All ? textColor : Color.Red,
                    FontFlags.None);
            }

            if (Menu.EnabledBkb)
            {
                Drawing.DrawText(BkbText, Font, bkbTextPosition, textSize, textColor, FontFlags.None);
            }

            if (sleeper.Sleeping("avoiding") || sleeper.Sleeping("block"))
            {
                Drawing.DrawText(
                    MovementBlockedText,
                    Font,
                    movementBlockedPosition,
                    textSize,
                    Color.Red,
                    FontFlags.None);
            }
        }
    }
}
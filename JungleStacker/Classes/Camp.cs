namespace JungleStacker.Classes
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Objects;

    using SharpDX;

    using Utils;

    internal class Camp : JungleCamp
    {
        private const uint WmMousewheel = 0x020A;

        private static readonly DotaTexture DecreaseArrow = Textures.GetTexture("materials/ensage_ui/other/arrow_usual");

        private static readonly DotaTexture IncreaseArrow =
            Textures.GetTexture("materials/ensage_ui/other/arrow_usual_left");

        private Vector2 arrowRectangleSize;

        private float campNameTextPosition;

        private Vector2 countTextSize;

        private Vector2 position;

        private Vector2 pullTimeTextSize;

        private int requiredStacksCount = 1;

        private Vector2 requiredStacksTextSize;

        private Vector2 stacksTextSize;

        private Vector2 stackTimeTextSize;

        public Camp()
        {
            requiredStacksTextSize =
                Drawing.MeasureText("Required stacks:", "Arial", new Vector2(15), FontFlags.None) - 18;
            stacksTextSize = Drawing.MeasureText("Stacks:", "Arial", new Vector2(15), FontFlags.None) - 5;
            pullTimeTextSize = Drawing.MeasureText("Pull time:", "Arial", new Vector2(15), FontFlags.None) + 10;
            stackTimeTextSize = Drawing.MeasureText("Stack time:", "Arial", new Vector2(15), FontFlags.None) + 45;
            countTextSize = Drawing.MeasureText("9", "Arial", new Vector2(16), FontFlags.None);
            arrowRectangleSize = new Vector2(20);

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnWndProc += Game_OnWndProc;
        }

        public static bool Debug { set; get; }

        public static bool DisplayOverlay { get; set; } = true;

        public int CurrentStacksCount { get; set; } = 0;

        public bool DrawPullTime { get; set; }

        public bool IsCleared { get; set; } = true;

        public bool IsStacking { get; set; }

        public double MaxTimeAdjustment { get; set; }

        public Vector3 OverlayPosition { get; set; }

        public string PullTime { get; set; }

        public int RequiredStacksCount
        {
            get
            {
                return requiredStacksCount;
            }
            set
            {
                requiredStacksCount = value > 9 ? 1 : value < 1 ? 9 : value;
            }
        }

        public int StackCountTimeAdjustment { get; set; }

        public double TimeAdjustment { get; set; }

        private float GetCampNamePosition
        {
            get
            {
                if (campNameTextPosition <= 0)
                {
                    campNameTextPosition = (OverlaySize.X - Drawing.MeasureText(
                                                    Name,
                                                    "Arial",
                                                    new Vector2(15),
                                                    FontFlags.None)
                                                .X) / 2;
                }

                return campNameTextPosition;
            }
        }

        private bool IsUnderBox => Utils.IsUnderRectangle(
            Game.MouseScreenPosition,
            position.X - 32,
            position.Y - 12,
            OverlaySize.X + 1,
            OverlaySize.Y + 1);

        private bool IsUnderDecreaseArrow => Utils.IsUnderRectangle(
            Game.MouseScreenPosition,
            position.X + requiredStacksTextSize.X + 5,
            position.Y + 38,
            arrowRectangleSize.X,
            arrowRectangleSize.Y);

        private bool IsUnderIncreaseArrow => Utils.IsUnderRectangle(
            Game.MouseScreenPosition,
            position.X + requiredStacksTextSize.X + countTextSize.X + arrowRectangleSize.X + 19,
            position.Y + 38,
            arrowRectangleSize.X,
            arrowRectangleSize.Y);

        private Vector2 OverlaySize
        {
            get
            {
                if (DrawPullTime)
                {
                    return new Vector2(180, 120);
                }

                return new Vector2(180, 100);
            }
        }

        public void OnClose()
        {
            Drawing.OnDraw -= Drawing_OnDraw;
            Game.OnWndProc -= Game_OnWndProc;
            OnDisable();
        }

        public void OnDisable()
        {
            IsStacking = false;
        }

        public void ResetStacks()
        {
            requiredStacksCount = 1;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!DisplayOverlay)
            {
                return;
            }

            Drawing.WorldToScreen(OverlayPosition, out position);

            if (position.IsZero)
            {
                return;
            }

            position -= new Vector2(28, 85);
            var nameColor = requiredStacksCount > 1
                                ? requiredStacksCount > CurrentStacksCount
                                      ? Color.Yellow
                                      : Color.LightGreen
                                : Color.White;

            if (!IsUnderBox)
            {
                Drawing.DrawRect(
                    new Vector2(position.X - 30, position.Y - 11),
                    new Vector2(90, 30),
                    new Color(30, 30, 30, 100));

                Drawing.DrawRect(
                    new Vector2(position.X - 31, position.Y - 12),
                    new Vector2(91, 31),
                    new Color(0, 0, 0, 100),
                    true);

                var nameColorAlpha = new Color((int)nameColor.R, nameColor.G, nameColor.B, 200);

                Drawing.DrawText(
                    "Stacks:",
                    new Vector2(position.X - 20, position.Y - 5),
                    new Vector2(15),
                    nameColorAlpha,
                    FontFlags.None);

                Drawing.DrawText(
                    CurrentStacksCount + "/" + RequiredStacksCount,
                    position + new Vector2(stacksTextSize.X - 5, -5),
                    new Vector2(16),
                    nameColorAlpha,
                    FontFlags.None);

                return;
            }

            // box
            Drawing.DrawRect(new Vector2(position.X - 30, position.Y - 11), OverlaySize, new Color(30, 30, 30, 175));

            //border
            Drawing.DrawRect(
                new Vector2(position.X - 31, position.Y - 12),
                OverlaySize + new Vector2(1),
                new Color(0, 0, 0, 175),
                true);

            var campName = Name;

            if (Debug)
            {
                campName += " ID: " + Id;
            }

            // 1st line
            Drawing.DrawText(
                campName,
                new Vector2(position.X - 30 + GetCampNamePosition, position.Y - 3),
                new Vector2(15),
                nameColor,
                FontFlags.None);

            // 2nd line
            Drawing.DrawText(
                "Current stacks:",
                new Vector2(position.X - 20, position.Y + 20),
                new Vector2(15),
                Color.White,
                FontFlags.None);

            Drawing.DrawText(
                CurrentStacksCount.ToString(),
                position + new Vector2(requiredStacksTextSize.X + arrowRectangleSize.X + 12, 20),
                new Vector2(16),
                Color.White,
                FontFlags.None);

            // 3rd line
            Drawing.DrawText(
                "Required stacks:",
                new Vector2(position.X - 20, position.Y + 40),
                new Vector2(15),
                Color.White,
                FontFlags.None);

            // 4th line
            Drawing.DrawText(
                "Stack time:",
                new Vector2(position.X - 20, position.Y + 60),
                new Vector2(15),
                Color.White,
                FontFlags.None);

            Drawing.DrawText(
                StackTime.ToString(),
                position + new Vector2(stackTimeTextSize.X, 60),
                new Vector2(16),
                Color.White,
                FontFlags.None);

            if (DrawPullTime)
            {
                // 5th line
                Drawing.DrawText(
                    "Pull time:",
                    new Vector2(position.X - 20, position.Y + 80),
                    new Vector2(15),
                    Color.White,
                    FontFlags.None);

                Drawing.DrawText(
                    PullTime,
                    position + new Vector2(pullTimeTextSize.X + 12, 80),
                    new Vector2(16),
                    Color.White,
                    FontFlags.None);
            }

            var alphaDecArrow = IsUnderDecreaseArrow ? 25 : -25;

            DrawingUtils.RoundedRectangle(
                position.X + requiredStacksTextSize.X + 5,
                position.Y + 38,
                arrowRectangleSize.X,
                arrowRectangleSize.Y,
                4,
                new Color(100 + alphaDecArrow, 100 + alphaDecArrow, 100 + alphaDecArrow, 200 + alphaDecArrow / 2));

            Drawing.DrawRect(
                position + new Vector2(requiredStacksTextSize.X + 7, 41),
                new Vector2(16, 16),
                DecreaseArrow);

            Drawing.DrawText(
                RequiredStacksCount.ToString(),
                position + new Vector2(requiredStacksTextSize.X + arrowRectangleSize.X + 12, 40),
                new Vector2(16),
                Color.White,
                FontFlags.None);

            var alphaIncArrow = IsUnderIncreaseArrow ? 25 : -25;

            DrawingUtils.RoundedRectangle(
                position.X + requiredStacksTextSize.X + countTextSize.X + arrowRectangleSize.X + 19,
                position.Y + 38,
                arrowRectangleSize.X,
                arrowRectangleSize.Y,
                3,
                new Color(100 + alphaIncArrow, 100 + alphaIncArrow, 100 + alphaIncArrow, 200 + alphaIncArrow / 2));

            Drawing.DrawRect(
                position + new Vector2(requiredStacksTextSize.X + countTextSize.X + arrowRectangleSize.X + 22, 39),
                new Vector2(16, 16),
                IncreaseArrow);
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (!DisplayOverlay)
            {
                return;
            }

            if (args.Msg == WmMousewheel && IsUnderBox)
            {
                var delta = (short)((args.WParam >> 16) & 0xFFFF);
                if (delta > 0)
                {
                    RequiredStacksCount++;
                }
                else
                {
                    RequiredStacksCount--;
                }
                args.Process = false;
            }

            if (args.Msg != (ulong)Utils.WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }

            if (IsUnderIncreaseArrow)
            {
                RequiredStacksCount++;
            }
            else if (IsUnderDecreaseArrow)
            {
                RequiredStacksCount--;
            }
        }
    }
}
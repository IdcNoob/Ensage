namespace JungleStacker.Classes
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Objects;

    using global::JungleStacker.Utils;

    using SharpDX;

    internal class Camp : JungleCamp
    {
        #region Static Fields

        private static readonly DotaTexture DecreaseArrow = Textures.GetTexture("materials/ensage_ui/other/arrow_usual");

        private static readonly DotaTexture IncreaseArrow =
            Textures.GetTexture("materials/ensage_ui/other/arrow_usual_left");

        #endregion

        #region Fields

        private Vector2 arrowRectangleSize;

        private Vector2 countTextSize;

        private Vector2 position;

        private int requiredStacksCount;

        private Vector2 requiredStacksTextSize;

        private Vector2 stacksTextSize;

        #endregion

        #region Constructors and Destructors

        public Camp()
        {
            DisplayOverlay = true;

            this.RequiredStacksCount = 1;
            this.CurrentStacksCount = 0;
            this.IsCleared = true;

            this.requiredStacksTextSize = Drawing.MeasureText(
                "Stacks required:",
                "Arial",
                new Vector2(15),
                FontFlags.None) - 18;
            this.stacksTextSize = Drawing.MeasureText("Stacks:", "Arial", new Vector2(15), FontFlags.None) - 5;
            this.countTextSize = Drawing.MeasureText("9", "Arial", new Vector2(16), FontFlags.None);
            this.arrowRectangleSize = new Vector2(20);

            Drawing.OnDraw += this.Drawing_OnDraw;
            Game.OnWndProc += this.Game_OnWndProc;
        }

        #endregion

        #region Public Properties

        public static bool DisplayOverlay { get; set; }

        public int CurrentStacksCount { get; set; }

        public bool IsCleared { get; set; }

        public bool IsStacking { get; set; }

        public Vector3 OverlayPosition { get; set; }

        public int RequiredStacksCount
        {
            get
            {
                return this.requiredStacksCount;
            }

            set
            {
                this.requiredStacksCount = value > 9 ? 1 : value < 1 ? 9 : value;
            }
        }

        #endregion

        #region Properties

        private bool IsUnderDecreaseArrow
        {
            get
            {
                return Utils.IsUnderRectangle(
                    Game.MouseScreenPosition,
                    this.position.X + this.requiredStacksTextSize.X + 5,
                    this.position.Y - 2,
                    this.arrowRectangleSize.X,
                    this.arrowRectangleSize.Y);
            }
        }

        private bool IsUnderIncreaseArrow
        {
            get
            {
                return Utils.IsUnderRectangle(
                    Game.MouseScreenPosition,
                    this.position.X + this.requiredStacksTextSize.X + this.countTextSize.X + this.arrowRectangleSize.X
                    + 19,
                    this.position.Y - 2,
                    this.arrowRectangleSize.X,
                    this.arrowRectangleSize.Y);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            Drawing.OnDraw -= this.Drawing_OnDraw;
            Game.OnWndProc -= this.Game_OnWndProc;
        }

        #endregion

        #region Methods

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!DisplayOverlay)
            {
                return;
            }

            Drawing.WorldToScreen(this.OverlayPosition, out this.position);

            if (this.position.IsZero)
            {
                return;
            }

            this.position -= new Vector2(28, 85);

            if (!Utils.IsUnderRectangle(Game.MouseScreenPosition, this.position.X - 32, this.position.Y - 12, 175, 61))
            {
                Drawing.DrawRect(
                    new Vector2(this.position.X - 30, this.position.Y - 11),
                    new Vector2(90, 30),
                    new Color(30, 30, 30, 75));

                Drawing.DrawRect(
                    new Vector2(this.position.X - 31, this.position.Y - 12),
                    new Vector2(91, 31),
                    new Color(0, 0, 0, 75),
                    true);

                Drawing.DrawText(
                    "Stacks:",
                    new Vector2(this.position.X - 20, this.position.Y - 5),
                    new Vector2(15),
                    new Color(255, 255, 255, 150),
                    FontFlags.None);

                Drawing.DrawText(
                    this.CurrentStacksCount + "/",
                    this.position + new Vector2(this.stacksTextSize.X - 5, -5),
                    new Vector2(16),
                    new Color(220, 220, 220, 150),
                    FontFlags.None);

                Drawing.DrawText(
                    this.RequiredStacksCount.ToString(),
                    this.position + new Vector2(this.stacksTextSize.X + this.countTextSize.X, -5),
                    new Vector2(16),
                    new Color(220, 220, 220, 150),
                    FontFlags.None);

                return;
            }

            Drawing.DrawRect(
                new Vector2(this.position.X - 30, this.position.Y - 11),
                new Vector2(174, 60),
                new Color(30, 30, 30, 175));

            Drawing.DrawRect(
                new Vector2(this.position.X - 31, this.position.Y - 12),
                new Vector2(175, 61),
                new Color(0, 0, 0, 175),
                true);

            Drawing.DrawText(
                //"Stacks required:",
                //this.Id.ToString(),
                this.Name,
                new Vector2(this.position.X - 20, this.position.Y),
                new Vector2(15),
                Color.White,
                FontFlags.None);

            var alphaDecArrow = this.IsUnderDecreaseArrow ? 25 : -25;

            DrawingUtils.RoundedRectangle(
                this.position.X + this.requiredStacksTextSize.X + 5,
                this.position.Y - 2,
                this.arrowRectangleSize.X,
                this.arrowRectangleSize.Y,
                4,
                new Color(100 + alphaDecArrow, 100 + alphaDecArrow, 100 + alphaDecArrow, 200 + (alphaDecArrow / 2)));

            Drawing.DrawRect(
                this.position + new Vector2(this.requiredStacksTextSize.X + 7, 1),
                new Vector2(16, 16),
                DecreaseArrow);

            Drawing.DrawText(
                this.RequiredStacksCount.ToString(),
                this.position + new Vector2(this.requiredStacksTextSize.X + this.arrowRectangleSize.X + 12, 0),
                new Vector2(16),
                Color.White,
                FontFlags.None);

            var alphaIncArrow = this.IsUnderIncreaseArrow ? 25 : -25;

            DrawingUtils.RoundedRectangle(
                this.position.X + this.requiredStacksTextSize.X + this.countTextSize.X + this.arrowRectangleSize.X + 19,
                this.position.Y - 2,
                this.arrowRectangleSize.X,
                this.arrowRectangleSize.Y,
                3,
                new Color(100 + alphaIncArrow, 100 + alphaIncArrow, 100 + alphaIncArrow, 200 + (alphaIncArrow / 2)));

            Drawing.DrawRect(
                this.position
                + new Vector2(this.requiredStacksTextSize.X + this.countTextSize.X + this.arrowRectangleSize.X + 22, -1),
                new Vector2(16, 16),
                IncreaseArrow);

            Drawing.DrawText(
                "Current stacks:",
                new Vector2(this.position.X - 20, this.position.Y + 20),
                new Vector2(15),
                Color.White,
                FontFlags.None);

            Drawing.DrawText(
                this.CurrentStacksCount.ToString(),
                this.position + new Vector2(this.requiredStacksTextSize.X + this.arrowRectangleSize.X + 12, 20),
                new Vector2(16),
                Color.White,
                FontFlags.None);
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (ulong)Utils.WindowsMessages.WM_LBUTTONDOWN || !DisplayOverlay)
            {
                return;
            }

            if (this.IsUnderIncreaseArrow)
            {
                this.RequiredStacksCount++;
            }
            else if (this.IsUnderDecreaseArrow)
            {
                this.RequiredStacksCount--;
            }
        }

        #endregion
    }
}
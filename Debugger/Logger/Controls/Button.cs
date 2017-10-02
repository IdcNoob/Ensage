namespace Debugger.Logger.Controls
{
    using Ensage;
    using Ensage.Common;

    using SharpDX;

    internal class Button
    {
        private readonly Vector2 buttonSize;

        private readonly string name;

        private Vector2 position;

        private Vector2 textMeasuredSize;

        private Vector2 textPosition;

        private Vector2 textSize;

        public Button(string name, Vector2 position, Vector2 buttonSize)
        {
            this.name = name;
            this.position = position;
            this.buttonSize = buttonSize;
        }

        public virtual void Draw()
        {
            var backgroundColor = new Color(50, 50, 50, 200);
            Drawing.DrawRect(this.position, this.buttonSize, this.IsMouseUnderButton() ? backgroundColor.SetAlpha(255) : backgroundColor);
            Drawing.DrawText(this.name, "Arial", this.textPosition, this.textSize, Color.DeepSkyBlue, FontFlags.DropShadow);
        }

        public bool IsMouseUnderButton()
        {
            return Utils.IsUnderRectangle(Game.MouseScreenPosition, this.position.X, this.position.Y, this.buttonSize.X, this.buttonSize.Y);
        }

        public virtual void UpdateSize(float size)
        {
            this.textMeasuredSize = Drawing.MeasureText(this.name, "Arial", new Vector2(size + 1), FontFlags.DropShadow);
            this.textSize = new Vector2(size + 1);
            this.textPosition = new Vector2(
                this.position.X + ((this.buttonSize.X / 2) - (this.textMeasuredSize.X / 2)),
                this.position.Y + ((this.buttonSize.Y / 2) - (this.textMeasuredSize.Y / 2)));
        }

        public virtual void UpdateYPosition(float y)
        {
            this.position = new Vector2(this.position.X, y);
            this.UpdateSize(this.textSize.X - 1);
        }
    }
}
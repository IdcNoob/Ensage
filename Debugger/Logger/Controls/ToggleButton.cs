namespace Debugger.Logger.Controls
{
    using SharpDX;

    internal class ToggleButton : Button
    {
        private readonly Button disableButton;

        public ToggleButton(string name, string nameOff, Vector2 position, Vector2 buttonSize)
            : base(name, position, buttonSize)
        {
            this.disableButton = new Button(nameOff, position, buttonSize);
        }

        public bool Enabled { get; set; } = true;

        public override void Draw()
        {
            if (this.Enabled)
            {
                base.Draw();
            }
            else
            {
                this.disableButton.Draw();
            }
        }

        public override void UpdateSize(float size)
        {
            base.UpdateSize(size);
            this.disableButton.UpdateSize(size);
        }

        public override void UpdateYPosition(float y)
        {
            base.UpdateYPosition(y);
            this.disableButton.UpdateYPosition(y);
        }
    }
}
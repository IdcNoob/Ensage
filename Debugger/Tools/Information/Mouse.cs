namespace Debugger.Tools.Information
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Windows.Forms;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Mouse : IDebuggerTool
    {
        private const uint WM_LBUTTONDOWN = 0x0201;

        private MenuItem<bool> copyPosition;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        private MenuItem<bool> showMousePosition;

        public int LoadPriority { get; } = 77;

        public void Activate()
        {
            this.menu = this.mainMenu.InformationMenu.Menu("Mouse");

            this.showMousePosition = this.menu.Item("Show mouse position", false);
            this.showMousePosition.PropertyChanged += this.ShowMousePositionOnPropertyChanged;

            this.copyPosition = this.menu.Item("Copy position on click", true);
            this.copyPosition.PropertyChanged += this.CopyPositionOnPropertyChanged;

            this.ShowMousePositionOnPropertyChanged(null, null);
            this.CopyPositionOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.showMousePosition.PropertyChanged -= this.ShowMousePositionOnPropertyChanged;
            this.copyPosition.PropertyChanged -= this.CopyPositionOnPropertyChanged;
            Drawing.OnDraw -= this.DrawingOnDraw;
            Game.OnWndProc -= this.GameOnWndProc;
        }

        private void CopyPositionOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.copyPosition && this.showMousePosition)
            {
                Game.OnWndProc += this.GameOnWndProc;
            }
            else
            {
                Game.OnWndProc -= this.GameOnWndProc;
            }
        }

        private void DrawingOnDraw(EventArgs args)
        {
            var pos = Game.MousePosition;

            Drawing.DrawText(
                pos.ToCopyFormat(),
                "Arial",
                Game.MouseScreenPosition + new Vector2(35, 0),
                new Vector2(20),
                Color.White,
                FontFlags.DropShadow);
        }

        private void GameOnWndProc(WndEventArgs args)
        {
            if (args.Msg == WM_LBUTTONDOWN && !this.log.IsMouseUnderLog())
            {
                Clipboard.SetText(Game.MousePosition.ToCopyFormat());
            }
        }

        private void ShowMousePositionOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.showMousePosition)
            {
                Drawing.OnDraw += this.DrawingOnDraw;
            }
            else
            {
                Drawing.OnDraw -= this.DrawingOnDraw;
            }

            this.CopyPositionOnPropertyChanged(null, null);
        }
    }
}
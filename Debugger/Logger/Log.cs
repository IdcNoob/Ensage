namespace Debugger.Logger
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows.Forms;

    using Controls;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    using Menu;

    using SharpDX;

    using Tools;

    using Button = Controls.Button;

    [Export(typeof(ILog))]
    internal class Log : IDebuggerTool, ILog
    {
        private const uint WM_LBUTTONDOWN = 0x0201;

        private const uint WM_MOUSEWHEEL = 0x020A;

        private const uint WM_RBUTTONDOWN = 0x0204;

        private readonly List<LogItem> displayList = new List<LogItem>();

        private readonly List<LogItem> items = new List<LogItem>();

        private Button clearButton;

        private MenuItem<Slider> itemsToSave;

        private Button jumpTopButton;

        private MenuItem<Slider> linesToShow;

        [Import]
        private IMenu mainMenu;

        private ToggleButton overlayButton;

        private ToggleButton pauseButton;

        private MenuItem<Slider> positionX;

        private MenuItem<Slider> positionY;

        private float screenSizeX;

        private int scrollPosition;

        private MenuItem<Slider> textSize;

        public int LoadPriority { get; } = 999;

        private int ScrollPosition
        {
            get
            {
                return this.scrollPosition;
            }

            set
            {
                if (value <= 0)
                {
                    this.scrollPosition = 0;
                }
                else if (value >= this.items.Count)
                {
                    this.scrollPosition = this.items.Count - 1;
                }
                else
                {
                    this.scrollPosition = value;
                }
            }
        }

        public void Activate()
        {
            var menu = this.mainMenu.OverlaySettingsMenu;

            this.itemsToSave = menu.Item("Items to save", new Slider(100, 5, 500));
            this.linesToShow = menu.Item("Lines to show", new Slider(30, 10, 50));
            this.linesToShow.PropertyChanged += this.LinesToShowOnPropertyChanged;
            this.textSize = menu.Item("Text size", new Slider(20, 10, 30));
            this.textSize.PropertyChanged += this.TextSizeOnPropertyChanged;
            this.screenSizeX = HUDInfo.ScreenSizeX();
            this.positionX = menu.Item("Position x", new Slider((int)(this.screenSizeX * 0.75), 0, (int)this.screenSizeX));
            this.positionY = menu.Item("Position y", new Slider(100, 0, (int)HUDInfo.ScreenSizeY()));
            this.positionY.PropertyChanged += this.PositionYOnPropertyChanged;

            this.overlayButton = new ToggleButton(
                "Hide",
                "Show",
                new Vector2(this.screenSizeX - 100, this.positionY - 50),
                new Vector2(100, 30));
            this.pauseButton = new ToggleButton(
                "Pause",
                "Continue",
                new Vector2(this.screenSizeX - 200, this.positionY - 50),
                new Vector2(100, 30));
            this.clearButton = new Button("Clear", new Vector2(this.screenSizeX - 300, this.positionY - 50), new Vector2(100, 30));
            this.jumpTopButton = new Button("^", new Vector2(this.screenSizeX - 400, this.positionY - 50), new Vector2(100, 30));

            this.TextSizeOnPropertyChanged(null, null);

            Drawing.OnDraw += this.DrawingOnDraw;
            Game.OnWndProc += this.GameOnWndProc;
        }

        public void Display(LogItem newItem)
        {
            if (!this.pauseButton.Enabled)
            {
                return;
            }

            if (this.items.Count > this.itemsToSave)
            {
                this.items.RemoveAt(0);
            }

            this.items.Add(newItem);

            if (this.ScrollPosition > 0)
            {
                this.ScrollPosition++;
            }

            this.UpdateOverlay();
        }

        public void Dispose()
        {
            this.linesToShow.PropertyChanged -= this.LinesToShowOnPropertyChanged;
            this.positionY.PropertyChanged += this.PositionYOnPropertyChanged;
            Drawing.OnDraw -= this.DrawingOnDraw;
            Game.OnWndProc -= this.GameOnWndProc;
        }

        public bool IsMouseUnderLog()
        {
            if (!this.overlayButton.Enabled)
            {
                return false;
            }

            return Utils.IsUnderRectangle(Game.MouseScreenPosition, this.positionX - 20, this.positionY - 20, 2000, 1000);
        }

        private void DrawingOnDraw(EventArgs args)
        {
            this.overlayButton.Draw();
            if (!this.overlayButton.Enabled)
            {
                return;
            }

            this.pauseButton.Draw();
            this.clearButton.Draw();

            if (this.scrollPosition > 0)
            {
                this.jumpTopButton.Draw();
            }

            var backgroundColor = new Color(50, 50, 50, 200);
            var totalSize = this.displayList.Sum(x => x.Lines.Count + 2);
            if (totalSize > 0)
            {
                Drawing.DrawRect(
                    new Vector2(this.positionX - 20, this.positionY - 20),
                    new Vector2(2000, (totalSize * this.textSize) + 20),
                    backgroundColor);
            }

            var selectedLine = (int)((Game.MouseScreenPosition.Y - this.positionY) / this.textSize);
            var offset = 0;

            foreach (var item in this.displayList)
            {
                if (!string.IsNullOrEmpty(item.FirstLine))
                {
                    Drawing.DrawText(
                        item.FirstLine,
                        "Arial",
                        new Vector2(this.positionX, this.positionY + (offset * this.textSize)),
                        new Vector2(this.textSize),
                        item.Color,
                        FontFlags.DropShadow);
                }

                var startColor = item.Color;
                var endColor = Color.White;
                var time = Math.Min(Game.RawGameTime - item.Time, 1);

                var color = new Color(
                    (int)(((endColor.R - startColor.R) * time) + startColor.R),
                    (int)(((endColor.G - startColor.G) * time) + startColor.G),
                    (int)(((endColor.B - startColor.B) * time) + startColor.B));

                foreach (var itemLine in item.Lines)
                {
                    offset++;
                    Drawing.DrawText(
                        itemLine.Item1,
                        "Arial",
                        new Vector2(this.positionX, this.positionY + (offset * this.textSize)),
                        new Vector2(this.textSize),
                        selectedLine == offset && this.IsMouseUnderLog() ? Color.Orange : color,
                        FontFlags.DropShadow);
                }

                offset += 2;
            }
        }

        private void GameOnWndProc(WndEventArgs args)
        {
            if (args.Msg == WM_MOUSEWHEEL && this.IsMouseUnderLog())
            {
                var delta = (short)((args.WParam >> 16) & 0xFFFF);
                if (delta > 0)
                {
                    this.ScrollPosition--;
                }
                else
                {
                    this.ScrollPosition++;
                }

                this.UpdateOverlay();
                args.Process = false;
                return;
            }

            if (args.Msg == WM_LBUTTONDOWN)
            {
                if (this.overlayButton.IsMouseUnderButton())
                {
                    this.overlayButton.Enabled = !this.overlayButton.Enabled;
                    args.Process = false;
                    return;
                }

                if (this.jumpTopButton.IsMouseUnderButton())
                {
                    this.scrollPosition = 0;
                    this.UpdateOverlay();
                    args.Process = false;
                    return;
                }

                if (this.clearButton.IsMouseUnderButton())
                {
                    this.scrollPosition = 0;
                    this.items.Clear();
                    this.UpdateOverlay();
                    args.Process = false;
                    return;
                }

                if (this.pauseButton.IsMouseUnderButton())
                {
                    this.pauseButton.Enabled = !this.pauseButton.Enabled;
                    args.Process = false;
                    return;
                }

                if (this.IsMouseUnderLog())
                {
                    var line = (int)((Game.MouseScreenPosition.Y - this.positionY) / this.textSize);
                    var offset = 0;

                    foreach (var item in this.displayList)
                    {
                        foreach (var itemLine in item.Lines)
                        {
                            if (++offset == line)
                            {
                                if (!string.IsNullOrEmpty(itemLine.Item2))
                                {
                                    Clipboard.SetText(itemLine.Item2);
                                }

                                args.Process = false;
                                return;
                            }
                        }

                        offset += 2;
                    }
                }
            }
        }

        private void LinesToShowOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.UpdateOverlay();
        }

        private void PositionYOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.overlayButton.UpdateYPosition(this.positionY - 50);
            this.jumpTopButton.UpdateYPosition(this.positionY - 50);
            this.clearButton.UpdateYPosition(this.positionY - 50);
            this.pauseButton.UpdateYPosition(this.positionY - 50);
        }

        private void TextSizeOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.overlayButton.UpdateSize(this.textSize);
            this.jumpTopButton.UpdateSize(this.textSize);
            this.clearButton.UpdateSize(this.textSize);
            this.pauseButton.UpdateSize(this.textSize);
        }

        private void UpdateOverlay()
        {
            this.displayList.Clear();
            var totalLines = 0;

            foreach (var item in this.items.Reverse<LogItem>().Skip(this.scrollPosition))
            {
                this.displayList.Add(item);
                totalLines += item.Lines.Count + 2;

                if (totalLines >= this.linesToShow)
                {
                    break;
                }
            }
        }
    }
}
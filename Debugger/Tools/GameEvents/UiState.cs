namespace Debugger.Tools.GameEvents
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class UiState : IDebuggerTool
    {
        private MenuItem<bool> enabled;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        public int LoadPriority { get; } = 83;

        public void Activate()
        {
            this.menu = this.mainMenu.GameEventsMenu.Menu("UI state");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Game.OnUIStateChanged");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Game.OnUIStateChanged -= this.OnUIStateChanged;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Game.OnUIStateChanged += this.OnUIStateChanged;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Game.OnUIStateChanged -= this.OnUIStateChanged;
            }
        }

        private void OnUIStateChanged(UIStateChangedEventArgs args)
        {
            var item = new LogItem(LogType.GameEvent, Color.Yellow, "UI state changed");

            item.AddLine("Name: " + args.UIState, args.UIState);

            this.log.Display(item);
        }
    }
}
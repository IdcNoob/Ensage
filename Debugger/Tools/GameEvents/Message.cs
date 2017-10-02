namespace Debugger.Tools.GameEvents
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Message : IDebuggerTool
    {
        private MenuItem<bool> enabled;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        public int LoadPriority { get; } = 82;

        public void Activate()
        {
            this.menu = this.mainMenu.GameEventsMenu.Menu("Message");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Game.OnMessage");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Game.OnMessage -= this.OnMessage;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Game.OnMessage += this.OnMessage;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Game.OnMessage -= this.OnMessage;
            }
        }

        private void OnMessage(MessageEventArgs args)
        {
            var item = new LogItem(LogType.GameEvent, Color.Yellow, "Message");

            item.AddLine("Name: " + args.Message, args.Message);

            this.log.Display(item);
        }
    }
}
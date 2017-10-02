namespace Debugger.Tools.GameEvents
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class GcMessage : IDebuggerTool
    {
        private MenuItem<bool> enabled;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        public int LoadPriority { get; } = 84;

        public void Activate()
        {
            this.menu = this.mainMenu.GameEventsMenu.Menu("GC message");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Game.OnGCMessageReceive");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Game.OnGCMessageReceive -= this.OnGCMessageReceive;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Game.OnGCMessageReceive += this.OnGCMessageReceive;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Game.OnGCMessageReceive -= this.OnGCMessageReceive;
            }
        }

        private void OnGCMessageReceive(GCMessageEventArgs args)
        {
            var item = new LogItem(LogType.GameEvent, Color.Yellow, "GC message received");

            item.AddLine("Name: " + args.MessageID, args.MessageID);

            this.log.Display(item);
        }
    }
}
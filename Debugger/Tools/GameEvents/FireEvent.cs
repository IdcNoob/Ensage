namespace Debugger.Tools.GameEvents
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class FireEvent : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "dota_action_success"
        };

        private MenuItem<bool> enabled;

        private MenuItem<bool> ignoreUseless;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        public int LoadPriority { get; } = 85;

        public void Activate()
        {
            this.menu = this.mainMenu.GameEventsMenu.Menu("Fire event");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Game.OnFireEvent");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.ignoreUseless = this.menu.Item("Ignore useless", true);

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Game.OnFireEvent -= this.GameOnFireEvent;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Game.OnFireEvent += this.GameOnFireEvent;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Game.OnFireEvent -= this.GameOnFireEvent;
            }
        }

        private void GameOnFireEvent(FireEventEventArgs args)
        {
            if (!this.IsValid(args))
            {
                return;
            }

            var item = new LogItem(LogType.GameEvent, Color.Yellow, "Fire event");

            item.AddLine("Name: " + args.GameEvent.Name, args.GameEvent.Name);

            this.log.Display(item);
        }

        private bool IsValid(FireEventEventArgs args)
        {
            if (this.ignoreUseless && this.ignored.Contains(args.GameEvent.Name))
            {
                return false;
            }

            return true;
        }
    }
}
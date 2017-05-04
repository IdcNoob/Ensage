namespace Evader.Core.Menus
{
    using Ensage.Common.Menu;

    internal class HotkeysMenu
    {
        private Pathfinder.EvadeMode pathfinderMode;

        public HotkeysMenu(Menu rootMenu)
        {
            var menu = new Menu("Hotkeys", "hotkeys");

            var enabledEvader =
                new MenuItem("enable", "Enable evader").SetValue(new KeyBind(36, KeyBindType.Toggle, true));
            menu.AddItem(enabledEvader);
            enabledEvader.ValueChanged += (sender, args) => EnabledEvader = args.GetNewValue<KeyBind>().Active;
            EnabledEvader = enabledEvader.IsActive();

            var enabledPathfinder = new MenuItem("changePathfinder", "Change pathfinder mode")
                .SetValue(new KeyBind(35, KeyBindType.Press))
                .SetTooltip("Colors: orange - evade all; red - evade only disables");
            menu.AddItem(enabledPathfinder);
            enabledPathfinder.ValueChanged += (sender, args) =>
                {
                    if (args.GetNewValue<KeyBind>().Active)
                    {
                        PathfinderMode++;
                    }
                };

            var enabledBkb =
                new MenuItem("enableBkb", "Enable bkb").SetValue(new KeyBind('-', KeyBindType.Toggle, true));
            enabledBkb.SetTooltip("Enables/disables bkb usage with a hotkey");
            menu.AddItem(enabledBkb);
            enabledBkb.ValueChanged += (sender, args) => EnabledBkb = args.GetNewValue<KeyBind>().Active;
            EnabledBkb = enabledBkb.IsActive();

            menu.AddItem(
                    new MenuItem("forceBlink", "Force blink").SetValue(new KeyBind(46, KeyBindType.Press))
                        .SetTooltip("Blink in front of your hero as soon as possible"))
                .ValueChanged += (sender, args) => ForceBlink = args.GetNewValue<KeyBind>().Active;

            rootMenu.AddSubMenu(menu);
        }

        public bool EnabledBkb { get; private set; }

        public bool EnabledEvader { get; private set; }

        public bool ForceBlink { get; private set; }

        public Pathfinder.EvadeMode PathfinderMode
        {
            get
            {
                return pathfinderMode;
            }
            private set
            {
                pathfinderMode = value > Pathfinder.EvadeMode.None ? 0 : value;
            }
        }
    }
}
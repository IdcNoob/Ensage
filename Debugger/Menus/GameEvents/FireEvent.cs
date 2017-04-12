namespace Debugger.Menus.GameEvents
{
    using Ensage.Common.Menu;

    internal class FireEvent
    {
        public FireEvent(Menu mainMenu)
        {
            var menu = new Menu("Fire event", "fireEv");

            var enabled = new MenuItem("fireEvEnabled", "Enabled").SetValue(false).SetTooltip("Game.OnFireEvent");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => Enabled = args.GetNewValue<bool>();
            enabled.ValueChanged += (sender, args) =>
                {
                    Enabled = args.GetNewValue<bool>();
                    if (Enabled)
                    {
                        menu.DisplayName = menu.DisplayName += "*";
                    }
                    else
                    {
                        menu.DisplayName = menu.DisplayName.TrimEnd('*');
                    }
                };
            Enabled = enabled.IsActive();
            if (Enabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var ignoreUseless = new MenuItem("ignoreFireEv", "Ignore useless events").SetValue(false);
            menu.AddItem(ignoreUseless);
            ignoreUseless.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreUseless.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool Enabled { get; private set; }

        public bool IgnoreUseless { get; private set; }
    }
}
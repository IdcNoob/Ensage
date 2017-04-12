namespace Debugger.Menus.GameEvents
{
    using Ensage.Common.Menu;

    internal class GcMessage
    {
        public GcMessage(Menu mainMenu)
        {
            var menu = new Menu("GC message", "gcMess");

            var enabled = new MenuItem("GcmessEnabled", "Enabled").SetValue(false)
                .SetTooltip("Game.OnGCMessageReceive");
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

            mainMenu.AddSubMenu(menu);
        }

        public bool Enabled { get; private set; }
    }
}
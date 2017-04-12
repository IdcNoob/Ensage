namespace Debugger.Menus.GameEvents
{
    using Ensage.Common.Menu;

    internal class Message
    {
        public Message(Menu mainMenu)
        {
            var menu = new Menu("Message", "message");

            var enabled = new MenuItem("messageEnabled", "Enabled").SetValue(false).SetTooltip("Game.OnMessage");
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
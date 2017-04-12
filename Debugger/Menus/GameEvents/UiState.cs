namespace Debugger.Menus.GameEvents
{
    using Ensage.Common.Menu;

    internal class UiState
    {
        public UiState(Menu mainMenu)
        {
            var menu = new Menu("UI state", "uiState");

            var enabled = new MenuItem("UiStateEnabled", "Enabled").SetValue(false).SetTooltip("Game.OnUIStateChanged");
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
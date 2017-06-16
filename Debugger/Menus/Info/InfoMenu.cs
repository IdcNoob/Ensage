namespace Debugger.Menus.Info
{
    using Ensage.Common.Menu;

    internal class InfoMenu
    {
        public InfoMenu(Menu mainMenu)
        {
            var menu = new Menu("Information", "info");

            var showMousePosition = new MenuItem("infoMousePosition", "Game mouse position").SetValue(false);
            menu.AddItem(showMousePosition);
            showMousePosition.ValueChanged += (sender, args) => ShowGameMousePosition = args.GetNewValue<bool>();
            ShowGameMousePosition = showMousePosition.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool ShowGameMousePosition { get; private set; }
    }
}
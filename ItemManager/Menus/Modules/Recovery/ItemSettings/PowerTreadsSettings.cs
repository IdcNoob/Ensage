namespace ItemManager.Menus.Modules.Recovery.ItemSettings
{
    using Ensage.Common.Menu;

    internal class PowerTreadsSettings
    {
        public PowerTreadsSettings(Menu mainMenu)
        {
            var menu = new Menu("Power treads", "powerTreadsSettings");

            var enabled = new MenuItem("powerTreadsRecovery", "Switch").SetValue(true);
            enabled.SetTooltip("Switch power treads when abuse enabled");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool IsEnabled { get; private set; }
    }
}
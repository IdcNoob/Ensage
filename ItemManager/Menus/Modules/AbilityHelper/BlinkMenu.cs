namespace ItemManager.Menus.Modules.AbilityHelper
{
    using Ensage.Common.Menu;

    internal class BlinkMenu
    {
        public BlinkMenu(Menu mainMenu)
        {
            var menu = new Menu("Blink dagger", "blinkAdjustment");

            var enabled = new MenuItem("blinkAdjustment", "Maximize blink range").SetValue(true);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool IsEnabled { get; private set; }
    }
}
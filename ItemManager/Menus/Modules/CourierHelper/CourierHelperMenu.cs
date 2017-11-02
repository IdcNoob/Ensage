namespace ItemManager.Menus.Modules.CourierHelper
{
    using Ensage.Common.Menu;

    internal class CourierHelperMenu
    {
        public CourierHelperMenu(Menu mainMenu)
        {
            var menu = new Menu("Courier helper", "courierHelper");

            var autoControl = new MenuItem("courierAuto", "Auto control").SetValue(true);
            autoControl.SetTooltip("Auto resend and reuse courier");
            menu.AddItem(autoControl);
            autoControl.ValueChanged += (sender, args) => AutoControl = args.GetNewValue<bool>();
            AutoControl = autoControl.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool AutoControl { get; private set; }
    }
}
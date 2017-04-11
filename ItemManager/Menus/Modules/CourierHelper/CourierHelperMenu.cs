namespace ItemManager.Menus.Modules.CourierHelper
{
    using System;

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

            var bottleAbuse =
                new MenuItem("courierBottleAbuse", "Bottle abuse").SetValue(new KeyBind('Z', KeyBindType.Press));
            bottleAbuse.SetTooltip("Courier will take your bottle and refill it");
            menu.AddItem(bottleAbuse);
            bottleAbuse.ValueChanged += (sender, args) =>
                {
                    if (args.GetNewValue<KeyBind>().Active)
                    {
                        OnBottleAbuse?.Invoke(this, EventArgs.Empty);
                    }
                };

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler OnBottleAbuse;

        public bool AutoControl { get; private set; }
    }
}
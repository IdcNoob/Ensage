namespace ItemManager.Menus.Modules.PtSwitcher
{
    using Ensage.Common.Menu;

    internal class PowerTreadsSwitcherMenu
    {
        public PowerTreadsSwitcherMenu(Menu mainMenu)
        {
            var menu = new Menu("Power treads switcher", "ptSwitcherMenu");

            var enabled = new MenuItem("ptSwitcherEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto switch power treads when using abilities");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsActive = args.GetNewValue<bool>();
            IsActive = enabled.IsActive();

            var switchOnHeal = new MenuItem("switchOnHeal", "Switch when healing").SetValue(true);
            switchOnHeal.SetTooltip("Bottle, flask, tango and some hero spells");
            menu.AddItem(switchOnHeal);
            switchOnHeal.ValueChanged += (sender, args) => SwitchOnHeal = args.GetNewValue<bool>();
            SwitchOnHeal = switchOnHeal.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool IsActive { get; private set; }

        public bool ItemsToBackpack { get; private set; }

        public bool SwitchOnHeal { get; private set; }
    }
}
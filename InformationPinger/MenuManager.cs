namespace InformationPinger
{
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem abilities;

        private readonly MenuItem courierUpgradeDelay;

        private readonly MenuItem courierUpgradeReminder;

        private readonly MenuItem enabled;

        private readonly MenuItem items;

        private readonly MenuItem itemWards;

        private readonly MenuItem roshanKillTime;

        private readonly MenuItem runeAutoDisable;

        private readonly MenuItem runeReminder;

        private readonly MenuItem runeReminderTime;

        private readonly MenuItem wardsDelay;

        private readonly MenuItem wardsReminder;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            var menu = new Menu("Info Pinger", "infoPinger", true);

            var itemMenu = new Menu("Enemy items", "items");
            itemMenu.AddItem(items = new MenuItem("itemsEnabled", "Enabled").SetValue(true));
            itemMenu.AddItem(itemWards = new MenuItem("wardsEnabled", "Ping wards").SetValue(false));

            var abilityMenu = new Menu("Enemy abilities", "abilities");
            abilityMenu.AddItem(abilities = new MenuItem("abilitiesEnabled", "Enabled").SetValue(true));

            var roshanMenu = new Menu("Roshan", "roshan");
            roshanMenu.AddItem(roshanKillTime = new MenuItem("roshanEnabled", "Kill time").SetValue(true));

            var runeMenu = new Menu("Runes", "runes");
            runeMenu.AddItem(runeReminder = new MenuItem("runeReminder", "Time reminder").SetValue(true));
            runeMenu.AddItem(
                runeReminderTime =
                new MenuItem("runeReminderTime", "Secs before rune spawn").SetValue(new Slider(10, 0, 30)));
            runeMenu.AddItem(
                runeAutoDisable =
                new MenuItem("runeAutoDisable", "Auto disable after X mins").SetValue(new Slider(10, 0, 60)));

            var wardsMenu = new Menu("Wards", "wards");
            wardsMenu.AddItem(wardsReminder = new MenuItem("wards", "Wards reminder").SetValue(true));
            wardsMenu.AddItem(
                wardsDelay = new MenuItem("wardsDelay", "Delay between pings in mins").SetValue(new Slider(5, 1, 20)));

            var courierMenu = new Menu("Courier", "courier");
            courierMenu.AddItem(courierUpgradeReminder = new MenuItem("upgrade", "Upgrade reminder").SetValue(true));
            courierMenu.AddItem(
                courierUpgradeDelay =
                new MenuItem("upgradeDelay", "Delay between pings in mins").SetValue(new Slider(5, 1, 20)));

            menu.AddItem(enabled = new MenuItem("enabled", "Enabled").SetValue(true));

            menu.AddSubMenu(itemMenu);
            menu.AddSubMenu(abilityMenu);
            menu.AddSubMenu(roshanMenu);
            menu.AddSubMenu(runeMenu);
            menu.AddSubMenu(wardsMenu);
            menu.AddSubMenu(courierMenu);

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool AbilityPingEnabled => abilities.IsActive();

        public int CourierUpgradeDelay => courierUpgradeDelay.GetValue<Slider>().Value;

        public bool CourierUpgradeReminder => courierUpgradeReminder.IsActive();

        public bool Enabled => enabled.IsActive();

        public bool ItemPingEnabled => items.IsActive();

        public bool ItemWardsEnabled => itemWards.IsActive();

        public bool RoshanKillTimeEnabled => roshanKillTime.IsActive();

        public int RuneAutoDisableTime => runeAutoDisable.GetValue<Slider>().Value;

        public bool RuneReminderEnabled => runeReminder.IsActive();

        public int RuneReminderTime => runeReminderTime.GetValue<Slider>().Value;

        public int WardsDelay => wardsDelay.GetValue<Slider>().Value;

        public bool WardsReminderEnabled => wardsReminder.IsActive();

        #endregion
    }
}
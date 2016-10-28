namespace InformationPinger
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem abilities;

        private readonly MenuItem abilityEnemyCheck;

        private readonly MenuItem bottleRune;

        private readonly MenuItem courierUpgradeDelay;

        private readonly MenuItem courierUpgradeReminder;

        private readonly MenuItem doubleAbilityPing;

        private readonly MenuItem doubleItemPing;

        private readonly MenuItem enabled;

        private readonly MenuItem forcePing;

        private readonly MenuItem itemCostGoldThreshold;

        private readonly MenuItem itemEnemyCheck;

        private readonly MenuItem items;

        private readonly MenuItem itemStatusCheck;

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
            itemMenu.AddItem(
                itemCostGoldThreshold = new MenuItem("itemCost", "Item cost").SetValue(new Slider(1800, 99, 5000)))
                .SetTooltip("Will ping items that costs more");
            itemMenu.AddItem(doubleItemPing = new MenuItem("itemsDoublePing", "Double ping").SetValue(false))
                .SetTooltip("Will ping item 2 times, like most people do");
            itemMenu.AddItem(
                forcePing =
                new MenuItem("forcePing", "Force ping:").SetValue(
                    new AbilityToggler(
                    new Dictionary<string, bool>
                        {
                            { "item_smoke_of_deceit", true },
                            { "item_dust", true },
                            { "item_gem", true },
                            { "item_bottle", true },
                            { "item_ward_dispenser", true },
                            { "item_ward_sentry", true },
                            { "item_ward_observer", true }
                        })));
            itemMenu.AddItem(
                bottleRune =
                new MenuItem("bottleRune", "Bottled rune:").SetValue(
                    new AbilityToggler(
                    new Dictionary<string, bool>
                        {
                            { "item_bottle_illusion", false },
                            { "item_bottle_regeneration", true },
                            { "item_bottle_arcane", true },
                            { "item_bottle_invisibility", true },
                            { "item_bottle_doubledamage", true },
                            { "item_bottle_haste", true }
                        })));
            itemMenu.AddItem(itemEnemyCheck = new MenuItem("itemEnemyCheck", "Check for enemies").SetValue(false))
                .SetTooltip("If there is any enemy hero/creep near you it wont ping, unless it's pinged enemy");
            itemMenu.AddItem(itemStatusCheck = new MenuItem("itemStatusCheck", "Check enemy status").SetValue(true))
                .SetTooltip("Delay ping if enemy is died or gone invisible");

            var abilityMenu = new Menu("Enemy abilities", "abilities");
            abilityMenu.AddItem(abilities = new MenuItem("abilitiesEnabled", "Enabled").SetValue(true));
            abilityMenu.AddItem(doubleAbilityPing = new MenuItem("abilitiesDoublePing", "Double ping").SetValue(false))
                .SetTooltip("Will ping ability 2 times, like most people do");
            abilityMenu.AddItem(
                abilityEnemyCheck = new MenuItem("abilityEnemyCheck", "Check for enemies").SetValue(false))
                .SetTooltip("If there is any enemy hero/creep near you it wont ping");

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

        public bool AbilityEnemyCheckEnabled => abilityEnemyCheck.IsActive();

        public bool AbilityPingEnabled => abilities.IsActive();

        public int CourierUpgradeDelay => courierUpgradeDelay.GetValue<Slider>().Value;

        public bool CourierUpgradeReminder => courierUpgradeReminder.IsActive();

        public bool DoubleAbilityPingEnabled => doubleAbilityPing.IsActive();

        public bool DoubleItemPingEnabled => doubleItemPing.IsActive();

        public bool Enabled => enabled.IsActive();

        public float ItemCostGoldThreshold => itemCostGoldThreshold.GetValue<Slider>().Value;

        public bool ItemEnemyCheckEnabled => itemEnemyCheck.IsActive();

        public bool ItemEnemyStatusEnabled => itemStatusCheck.IsActive();

        public bool ItemPingEnabled => items.IsActive();

        public bool RoshanKillTimeEnabled => roshanKillTime.IsActive();

        public int RuneAutoDisableTime => runeAutoDisable.GetValue<Slider>().Value;

        public bool RuneReminderEnabled => runeReminder.IsActive();

        public int RuneReminderTime => runeReminderTime.GetValue<Slider>().Value;

        public int WardsDelay => wardsDelay.GetValue<Slider>().Value;

        public bool WardsReminderEnabled => wardsReminder.IsActive();

        #endregion

        #region Public Methods and Operators

        public List<RuneType> BottleRunes()
        {
            var list = new List<RuneType>();

            foreach (var rune in bottleRune.GetValue<AbilityToggler>().Dictionary.Where(x => x.Value).Select(x => x.Key)
                )
            {
                switch (rune)
                {
                    case "item_bottle_illusion":
                        list.Add(RuneType.Illusion);
                        break;
                    case "item_bottle_regeneration":
                        list.Add(RuneType.Regeneration);
                        break;
                    case "item_bottle_arcane":
                        list.Add(RuneType.Arcane);
                        break;
                    case "item_bottle_invisibility":
                        list.Add(RuneType.Invisibility);
                        break;
                    case "item_bottle_doubledamage":
                        list.Add(RuneType.DoubleDamage);
                        break;
                    case "item_bottle_haste":
                        list.Add(RuneType.Haste);
                        break;
                }
            }

            return list;
        }

        public IEnumerable<string> ForcePingItems()
        {
            return forcePing.GetValue<AbilityToggler>().Dictionary.Where(x => x.Value).Select(x => x.Key).ToList();
        }

        #endregion
    }
}
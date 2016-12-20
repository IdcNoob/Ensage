namespace VisionControl
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem enabled;

        private readonly MenuItem enabledRanges;

        private readonly MenuItem enabledTimers;

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu("Vision Control", "visionControl", true);

            var unitsMenu = new Menu("Enabeld", "enabled");
            var rangesMenu = new Menu("Show ranges", "showRanges");
            var timersMenu = new Menu("Show timers", "showTimers");

            var units = Variables.Units.ToDictionary(x => x.Key, x => true);
            var ranges = new Dictionary<string, bool>(units);
            var timers = new Dictionary<string, bool>
            {
                { "techies_remote_mines", true },
                { "pugna_nether_ward", true },
                { "undying_tombstone", true },
                { "venomancer_plague_ward", true },
                { "item_ward_sentry", true },
                { "item_ward_observer", true }
            };

            unitsMenu.AddItem(enabled = new MenuItem("enabled", "For:").SetValue(new AbilityToggler(units)));

            rangesMenu.AddItem(
                enabledRanges = new MenuItem("enabledRanges", "For:").SetValue(new AbilityToggler(ranges)));

            timersMenu.AddItem(
                enabledTimers = new MenuItem("enabledTimers", "For:").SetValue(new AbilityToggler(timers)));

            menu.AddSubMenu(unitsMenu);
            menu.AddSubMenu(rangesMenu);
            menu.AddSubMenu(timersMenu);
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Methods and Operators

        public bool IsEnabled(string name)
        {
            return enabled.GetValue<AbilityToggler>().IsEnabled(name);
        }

        public bool IsEnabled(ClassID id)
        {
            var name = id == ClassID.CDOTA_NPC_Observer_Ward ? "item_ward_observer" : "item_ward_sentry";
            return IsEnabled(name);
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        public bool RangeEnabled(string name)
        {
            return enabledRanges.GetValue<AbilityToggler>().IsEnabled(name);
        }

        public bool TimerEnabled(string name)
        {
            return enabledTimers.GetValue<AbilityToggler>().IsEnabled(name);
        }

        #endregion
    }
}
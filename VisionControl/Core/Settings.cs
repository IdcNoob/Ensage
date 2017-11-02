namespace VisionControl.Core
{
    using System;
    using System.Collections.Generic;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuItem<AbilityToggler> enabled;

        private readonly MenuFactory factory;

        private readonly MenuItem<AbilityToggler> ranges;

        private readonly MenuItem<AbilityToggler> timers;

        public Settings()
        {
            factory = MenuFactory.Create("Vision Control");

            enabled = factory.Menu("Enabled")
                .Item(
                    "For: ",
                    new AbilityToggler(
                        new Dictionary<string, bool>
                        {
                            { "techies_land_mines", true },
                            { "techies_stasis_trap", true },
                            { "techies_remote_mines", true },
                            { "templar_assassin_trap", true },
                            { "pugna_nether_ward", true },
                            { "treant_eyes_in_the_forest", true },
                            { "undying_tombstone", true },
                            { "venomancer_plague_ward", true },
                            { "item_ward_sentry", true },
                            { "item_ward_observer", true }
                        }));

            ranges = factory.Menu("Show ranges")
                .Item(
                    "For: ",
                    new AbilityToggler(
                        new Dictionary<string, bool>
                        {
                            { "techies_land_mines", true },
                            { "techies_stasis_trap", true },
                            { "techies_remote_mines", true },
                            { "templar_assassin_trap", true },
                            { "pugna_nether_ward", true },
                            { "treant_eyes_in_the_forest", true },
                            { "undying_tombstone", true },
                            { "venomancer_plague_ward", true },
                            { "item_ward_sentry", true },
                            { "item_ward_observer", true }
                        }));

            timers = factory.Menu("Show timers")
                .Item(
                    "For: ",
                    new AbilityToggler(
                        new Dictionary<string, bool>
                        {
                            { "techies_remote_mines", true },
                            { "pugna_nether_ward", true },
                            { "undying_tombstone", true },
                            { "venomancer_plague_ward", true },
                            { "item_ward_sentry", true },
                            { "item_ward_observer", true }
                        }));

            PingWards = factory.Item("Ping enemy wards", true);
            PingWards.Item.Tooltip = "Ping is visible to your allies";
        }

        public MenuItem<bool> PingWards { get; }

        public void Dispose()
        {
            factory.Dispose();
        }

        public bool IsEnabled(string name)
        {
            return enabled.Value.IsEnabled(name);
        }

        public bool IsEnabled(AbilityId id)
        {
            return IsEnabled(id.ToString());
        }

        public bool RangeEnabled(string name)
        {
            return ranges.Value.IsEnabled(name);
        }

        public bool TimerEnabled(string name)
        {
            return timers.Value.IsEnabled(name);
        }
    }
}
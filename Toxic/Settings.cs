namespace Toxic
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings()
        {
            factory = MenuFactory.Create("TOXIC");
            Enabled = factory.Item("Enabled", false);
            Enabled.Item.SetTooltip("ONLY YOUR ALLIES WILL SEE PINGS, BUT NOT YOU!");
            DangerPing = factory.Item("Use danger ping type", true);
            Heroes = factory.Item("Ping:", new HeroToggler(new Dictionary<string, bool>(), false, true, false));
        }

        public MenuItem<bool> DangerPing { get; }

        public MenuItem<bool> Enabled { get; }

        public MenuItem<HeroToggler> Heroes { get; }

        public void Dispose()
        {
            factory?.Dispose();
        }
    }
}
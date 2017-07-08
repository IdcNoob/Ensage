namespace PredictedCreepsLocation.Core
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings()
        {
            factory = MenuFactory.Create("PCL");

            AlwaysEnabled = factory.Item("Always enabled", true);
            AlwaysEnabled.Item.Tooltip = "Always show positions or while key is pressed";
            ShowKey = factory.Item("Show key", new KeyBind(17));
            ShowCreepsCount = factory.Item("Show creep count", true);
            ShowOnMap = factory.Item("Show on map", true);
            ShowOnMinimap = factory.Item("Show on minimap", true);
        }

        public MenuItem<bool> AlwaysEnabled { get; }

        public MenuItem<bool> ShowCreepsCount { get; }

        public MenuItem<KeyBind> ShowKey { get; }

        public MenuItem<bool> ShowOnMap { get; }

        public MenuItem<bool> ShowOnMinimap { get; }

        public void Dispose()
        {
            factory?.Dispose();
        }
    }
}
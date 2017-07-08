namespace ExperienceTracker
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class MenuManager : IDisposable
    {
        private readonly MenuFactory factory;

        public MenuManager()
        {
            factory = MenuFactory.Create("Exp Tracker");

            Enabled = factory.Item("Enabled", true);
            WarningTime = factory.Item("Show warning for (sec)", new Slider(5, 1, 10));
            WarningSize = factory.Item("Warning size", new Slider(24, 10, 40));
            WarningX = factory.Item("Warning x position", new Slider(-15, -100));
            WarningY = factory.Item("Warning y position", new Slider(-25, -100));
            WarningRedColor = factory.Item("Warning red color", new Slider(255, 0, 255));
            WarningGreenColor = factory.Item("Warning green color", new Slider(200, 0, 255));
            WarningBlueColor = factory.Item("Warning blue color", new Slider(0, 0, 255));
            SimplifiedWarning = factory.Item("Simplified warning", false);
            SimplifiedWarning.Item.SetTooltip("Will show only number of enemies");
        }

        public MenuItem<bool> Enabled { get; }

        public MenuItem<bool> SimplifiedWarning { get; }

        public MenuItem<Slider> WarningBlueColor { get; }

        public MenuItem<Slider> WarningGreenColor { get; }

        public MenuItem<Slider> WarningRedColor { get; }

        public MenuItem<Slider> WarningSize { get; }

        public MenuItem<Slider> WarningTime { get; }

        public MenuItem<Slider> WarningX { get; }

        public MenuItem<Slider> WarningY { get; }

        public void Dispose()
        {
            factory.Dispose();
        }
    }
}
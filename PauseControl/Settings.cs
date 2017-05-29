namespace PauseControl
{
    using System;

    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings()
        {
            factory = MenuFactory.Create("Pause Control");

            Pause = factory.Item("Auto pause", true);
            Pause.Item.SetTooltip("Auto pause when ally is disconnected");

            Unpause = factory.Item("Force unpause", true);
            Unpause.Item.SetTooltip("Force unpause if pause is set by enemies");
        }

        public MenuItem<bool> Pause { get; }

        public MenuItem<bool> Unpause { get; }

        public void Dispose()
        {
            factory?.Dispose();
        }
    }
}
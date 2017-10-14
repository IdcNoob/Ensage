namespace PauseControl
{
    using System;

    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings()
        {
            this.factory = MenuFactory.Create("Pause Control");

            this.Pause = this.factory.Item("Auto pause", true);
            this.Pause.Item.SetTooltip("Auto pause when ally is disconnected");

            this.Unpause = this.factory.Item("Force unpause", true);
            this.Unpause.Item.SetTooltip("Force unpause if pause is set by enemies");
        }

        public MenuItem<bool> Pause { get; }

        public MenuItem<bool> Unpause { get; }

        public void Dispose()
        {
            this.factory?.Dispose();
        }
    }
}
namespace InformationPinger.Core
{
    using System.ComponentModel.Composition;

    using Ensage.SDK.Menu;

    using Interfaces;

    [Export(typeof(IMenuManager))]
    internal class MenuManager : IMenuManager
    {
        public MenuItem<bool> Enabled { get; private set; }

        public bool IsActive { get; private set; }

        public MenuFactory MenuFactory { get; private set; }

        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            MenuFactory = MenuFactory.Create("Info Pinger");
            Enabled = MenuFactory.Item("Enabled", true);
        }

        public void Dispose()
        {
            MenuFactory.Dispose();
            IsActive = false;
        }
    }
}
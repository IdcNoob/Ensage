namespace Debugger.Menu
{
    using System.ComponentModel.Composition;

    using Ensage.SDK.Menu;

    using SharpDX;

    using Tools;

    [Export(typeof(IMenu))]
    internal class Menu : IDebuggerTool, IMenu
    {
        private MenuFactory factory;

        public MenuFactory CheatsMenu { get; private set; }

        public MenuFactory GameEventsMenu { get; private set; }

        public MenuFactory InformationMenu { get; private set; }

        public int LoadPriority { get; } = 1000;

        public MenuFactory OnAddRemoveMenu { get; private set; }

        public MenuFactory OnChangeMenu { get; private set; }

        public MenuFactory OnExecuteOrderMenu { get; private set; }

        public MenuFactory OverlaySettingsMenu { get; private set; }

        public void Activate()
        {
            this.factory = MenuFactory.CreateWithTexture("Debugger", "chaos_knight_reality_rift");
            this.factory.Target.SetFontColor(Color.PaleVioletRed);

            this.OnAddRemoveMenu = this.factory.Menu("On add/remove");
            this.OnChangeMenu = this.factory.Menu("On change");
            this.InformationMenu = this.factory.Menu("Information");
            this.CheatsMenu = this.factory.Menu("Cheats");
            this.OnExecuteOrderMenu = this.factory.Menu("On execute order");
            this.GameEventsMenu = this.factory.Menu("Game events");
            this.OverlaySettingsMenu = this.factory.Menu("Overlay settings");
        }

        public void Dispose()
        {
            this.factory?.Dispose();
        }
    }
}
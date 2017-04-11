namespace ItemManager.Menus
{
    using Ensage.Common.Menu;

    using Modules.CourierHelper;
    using Modules.GoldSpender;
    using Modules.ItemSwap;
    using Modules.PtSwitcher;
    using Modules.Recovery;
    using Modules.ShrineHelper;
    using Modules.Snatcher;
    using Modules.SoulRing;

    internal class MenuManager
    {
        private readonly Menu mainMenu;

        public MenuManager()
        {
            mainMenu = new Menu(" Item Manager", "itemManager", true, "courier_go_to_secretshop", true);

            ItemSwapMenu = new ItemSwapMenu(mainMenu);
            CourierHelperMenu = new CourierHelperMenu(mainMenu);
            SnatcherMenu = new SnatcherMenu(mainMenu);
            GoldSpenderMenu = new GoldSpenderMenu(mainMenu);
            ShrineHelperMenu = new ShrineHelperMenu(mainMenu);
            PowerTreadsSwitcherMenu = new PowerTreadsSwitcherMenu(mainMenu);
            RecoveryMenu = new RecoveryMenu(mainMenu);
            SoulRingMenu = new SoulRingMenu(mainMenu);

            mainMenu.AddToMainMenu();
        }

        public CourierHelperMenu CourierHelperMenu { get; }

        public GoldSpenderMenu GoldSpenderMenu { get; }

        public ItemSwapMenu ItemSwapMenu { get; }

        public PowerTreadsSwitcherMenu PowerTreadsSwitcherMenu { get; }

        public RecoveryMenu RecoveryMenu { get; }

        public ShrineHelperMenu ShrineHelperMenu { get; }

        public SnatcherMenu SnatcherMenu { get; }

        public SoulRingMenu SoulRingMenu { get; }

        public void OnClose()
        {
            mainMenu.RemoveFromMainMenu();
        }
    }
}
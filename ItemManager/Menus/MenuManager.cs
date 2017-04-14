namespace ItemManager.Menus
{
    using System;

    using Ensage.Common.Menu;

    using Modules.AbilityHelper;
    using Modules.AutoUsage;
    using Modules.CourierHelper;
    using Modules.GoldSpender;
    using Modules.ItemSwap;
    using Modules.Recovery;
    using Modules.ShrineHelper;
    using Modules.Snatcher;

    internal class MenuManager : IDisposable
    {
        private readonly Menu mainMenu;

        public MenuManager()
        {
            mainMenu = new Menu(" Item Manager", "itemManager", true, "courier_go_to_secretshop", true);

            AutoUsageMenu = new AutoUsageMenu(mainMenu);
            AbilityHelperMenu = new AbilityHelperMenu(mainMenu);
            RecoveryMenu = new RecoveryMenu(mainMenu);
            GoldSpenderMenu = new GoldSpenderMenu(mainMenu);
            SnatcherMenu = new SnatcherMenu(mainMenu);
            ItemSwapMenu = new ItemSwapMenu(mainMenu);
            ShrineHelperMenu = new ShrineHelperMenu(mainMenu);
            CourierHelperMenu = new CourierHelperMenu(mainMenu);

            mainMenu.AddToMainMenu();
        }

        public AbilityHelperMenu AbilityHelperMenu { get; }

        public AutoUsageMenu AutoUsageMenu { get; }

        public CourierHelperMenu CourierHelperMenu { get; }

        public GoldSpenderMenu GoldSpenderMenu { get; }

        public ItemSwapMenu ItemSwapMenu { get; }

        public RecoveryMenu RecoveryMenu { get; }

        public ShrineHelperMenu ShrineHelperMenu { get; }

        public SnatcherMenu SnatcherMenu { get; }

        public void Dispose()
        {
            mainMenu.RemoveFromMainMenu();
        }
    }
}
namespace ItemManager.Menus
{
    using System;

    using Ensage.Common.Menu;

    using Modules.AbilityHelper;
    using Modules.AutoActions;
    using Modules.CourierHelper;
    using Modules.DefensiveAbilities;
    using Modules.GoldSpender;
    using Modules.ItemSwap;
    using Modules.OffensiveAbilities;
    using Modules.Recovery;
    using Modules.ShrineHelper;
    using Modules.Snatcher;

    internal class MenuManager : IDisposable
    {
        private readonly Menu mainMenu;

        public MenuManager()
        {
            mainMenu = new Menu(" Item Manager", "itemManager", true, "courier_go_to_secretshop", true);

            OffensiveAbilitiesMenu = new OffensiveAbilitiesMenu(mainMenu);
            DefensiveAbilitiesMenu = new DefensiveAbilitiesMenu(mainMenu);
            AutoActionsMenu = new AutoActionsMenu(mainMenu);
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

        public AutoActionsMenu AutoActionsMenu { get; }

        public CourierHelperMenu CourierHelperMenu { get; }

        public DefensiveAbilitiesMenu DefensiveAbilitiesMenu { get; }

        public GoldSpenderMenu GoldSpenderMenu { get; }

        public ItemSwapMenu ItemSwapMenu { get; }

        public OffensiveAbilitiesMenu OffensiveAbilitiesMenu { get; }

        public RecoveryMenu RecoveryMenu { get; }

        public ShrineHelperMenu ShrineHelperMenu { get; }

        public SnatcherMenu SnatcherMenu { get; }

        public void Dispose()
        {
            mainMenu.RemoveFromMainMenu();
        }
    }
}
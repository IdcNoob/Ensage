namespace Evader.Core.Menus
{
    using System;

    using Ensage.Common.Menu;

    internal class MenuManager : IDisposable
    {
        private readonly Menu menu;

        public MenuManager()
        {
            menu = new Menu("Evader", "evader", true, "techies_minefield_sign", true);

            EnemiesSettings = new EnemiesSettingsMenu(menu);
            AlliesSettings = new AlliesSettingsMenu(menu);
            UsableAbilities = new UsableAbilitiesMenu(menu);
            Hotkeys = new HotkeysMenu(menu);
            Settings = new SettingsMenu(menu);
            Randomiser = new RandomiserMenu(menu);
            Debug = new DebugMenu(menu);

            menu.AddToMainMenu();
        }

        public AlliesSettingsMenu AlliesSettings { get; }

        public DebugMenu Debug { get; }

        public EnemiesSettingsMenu EnemiesSettings { get; }

        public HotkeysMenu Hotkeys { get; }

        public RandomiserMenu Randomiser { get; }

        public SettingsMenu Settings { get; }

        public UsableAbilitiesMenu UsableAbilities { get; }

        public void Dispose()
        {
            menu.RemoveFromMainMenu();
        }
    }
}
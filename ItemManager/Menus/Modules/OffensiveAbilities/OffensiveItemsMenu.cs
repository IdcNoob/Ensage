namespace ItemManager.Menus.Modules.OffensiveAbilities
{
    using System.Collections.Generic;

    using AbilitySettings;

    using Ensage.Common.Menu;
    using Ensage.Common.Menu.MenuItems;

    internal class OffensiveItemsMenu
    {
        private readonly Dictionary<string, AbilitySettingsMenu> abilityMenus =
            new Dictionary<string, AbilitySettingsMenu>();

        private readonly Dictionary<string, bool> heroToggler = new Dictionary<string, bool>();

        private readonly Menu menu;

        public OffensiveItemsMenu(Menu mainMenu)
        {
            menu = new Menu("Offensive items", "offensiveItems");

            menu.AddItem(new EnemyHeroesToggler("OffItemsEnabledFor", "Use on:", heroToggler));

            mainMenu.AddSubMenu(menu);
        }

        public AbilitySettingsMenu CreateMenu(string name, string texture)
        {
            AbilitySettingsMenu abilitySettingsMenu;
            if (!abilityMenus.TryGetValue(name, out abilitySettingsMenu))
            {
                abilitySettingsMenu = new AbilitySettingsMenu(menu, name, texture);
                abilityMenus.Add(name, abilitySettingsMenu);
            }

            return abilitySettingsMenu;
        }

        public bool IsEnabled(string heroName)
        {
            bool enabled;
            heroToggler.TryGetValue(heroName, out enabled);
            return enabled;
        }
    }
}
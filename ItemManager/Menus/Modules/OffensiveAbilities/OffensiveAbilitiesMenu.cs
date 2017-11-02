namespace ItemManager.Menus.Modules.OffensiveAbilities
{
    using System.Collections.Generic;

    using AbilitySettings;

    using Core.Abilities.Interfaces;

    using Ensage.Common.Menu;
    using Ensage.Common.Menu.MenuItems;

    internal class OffensiveAbilitiesMenu
    {
        private readonly Dictionary<string, OffensiveAbilitySettings> abilityMenus = new Dictionary<string, OffensiveAbilitySettings>();

        private readonly Dictionary<string, bool> heroToggler = new Dictionary<string, bool>();

        private readonly Menu settingsMenu;

        private AbilityToggler abilityToggler;

        private PriorityChanger priorityChanger;

        public OffensiveAbilitiesMenu(Menu mainMenu)
        {
            var menu = new Menu("Offensive abilities", "offensiveItems");

            settingsMenu = new Menu("Settings", "offensiveItemsSettings");
            menu.AddSubMenu(settingsMenu);

            menu.AddItem(
                new MenuItem("enabledOffAbilities", "Enabled:").SetValue(
                    abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));
            menu.AddItem(
                new MenuItem("priorityOffAbilities", "Order:").SetValue(priorityChanger = new PriorityChanger(new List<string>())));

            menu.AddItem(new EnemyHeroesToggler("OffItemsEnabledFor", "Use on:", heroToggler));

            mainMenu.AddSubMenu(menu);
        }

        public void CreateMenu(IOffensiveAbility ability, string displayName)
        {
            OffensiveAbilitySettings abilitySettingsMenu;
            if (!abilityMenus.TryGetValue(displayName, out abilitySettingsMenu))
            {
                abilityToggler.Add(ability.Name, false);
                priorityChanger.Add(ability.Name);

                switch (ability.Name)
                {
                    case "item_satanic":
                        abilitySettingsMenu = new SatanicSettings(settingsMenu, displayName, ability.Name);
                        break;
                    default:
                        abilitySettingsMenu = new OffensiveAbilitySettings(settingsMenu, displayName, ability.Name);
                        break;
                }

                abilityMenus.Add(displayName, abilitySettingsMenu);
            }

            ability.Menu = abilitySettingsMenu;
        }

        public uint GetPriority(string abilityName)
        {
            return priorityChanger.GetPriority(abilityName);
        }

        public bool IsAbilityEnabled(string abilityName)
        {
            return abilityToggler.IsEnabled(abilityName);
        }

        public bool IsHeroEnabled(string heroName)
        {
            bool enabled;
            heroToggler.TryGetValue(heroName, out enabled);

            return enabled;
        }
    }
}
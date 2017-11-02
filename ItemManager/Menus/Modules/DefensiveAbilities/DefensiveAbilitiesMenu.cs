namespace ItemManager.Menus.Modules.DefensiveAbilities
{
    using System.Collections.Generic;

    using AbilitySettings;

    using Core.Abilities.Interfaces;

    using Ensage.Common.Menu;

    internal class DefensiveAbilitiesMenu
    {
        private readonly Dictionary<string, DefensiveAbilitySettings> abilityMenus = new Dictionary<string, DefensiveAbilitySettings>();

        private readonly Menu settingsMenu;

        private AbilityToggler abilityToggler;

        private PriorityChanger priorityChanger;

        public DefensiveAbilitiesMenu(Menu mainMenu)
        {
            var menu = new Menu("Defensive abilities", "defensiveItems");

            settingsMenu = new Menu("Settings", "DefensiveItemsSettings");
            menu.AddSubMenu(settingsMenu);

            menu.AddItem(
                new MenuItem("enabledDefAbilities", "Enabled:").SetValue(
                    abilityToggler = new AbilityToggler(new Dictionary<string, bool>())));
            menu.AddItem(
                new MenuItem("priorityDefAbilities", "Order:").SetValue(priorityChanger = new PriorityChanger(new List<string>())));

            mainMenu.AddSubMenu(menu);
        }

        public void CreateMenu(IDefensiveAbility ability, string displayName)
        {
            DefensiveAbilitySettings abilitySettingsMenu;
            if (!abilityMenus.TryGetValue(displayName, out abilitySettingsMenu))
            {
                abilityToggler.Add(ability.Name, false);
                priorityChanger.Add(ability.Name);
                abilitySettingsMenu = new DefensiveAbilitySettings(settingsMenu, displayName, ability.Name);
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
    }
}
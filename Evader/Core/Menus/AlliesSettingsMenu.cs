namespace Evader.Core.Menus
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.Common.Menu.MenuItems;

    internal class AlliesSettingsMenu
    {
        private readonly Dictionary<string, bool> allyToggler = new Dictionary<string, bool>();

        public AlliesSettingsMenu(Menu rootMenu)
        {
            var menu = new Menu("Allies settings", "alliesSettings");

            var helpAllies = new MenuItem("helpAllies", "Help allies").SetValue(false);
            menu.AddItem(helpAllies);
            helpAllies.ValueChanged += (sender, args) => HelpAllies = args.GetNewValue<bool>();
            HelpAllies = helpAllies.IsActive();

            menu.AddItem(new AllyHeroesToggler("enabledAllies", "Allies", allyToggler));

            var multiIntersection = new MenuItem("multiIntersectionDisable", "Multi intersection disable")
                .SetValue(false)
                .SetTooltip(
                    "Will disable enemy who's using AOE disable which will hit multiple allies (priority settings will be ignored)");
            menu.AddItem(multiIntersection);
            multiIntersection.ValueChanged +=
                (sender, args) => MultiIntersectionEnemyDisable = args.GetNewValue<bool>();
            MultiIntersectionEnemyDisable = multiIntersection.IsActive();

            rootMenu.AddSubMenu(menu);
        }

        public bool HelpAllies { get; private set; }

        public bool MultiIntersectionEnemyDisable { get; private set; }

        public bool Enabled(string heroName)
        {
            bool enabled;
            allyToggler.TryGetValue(heroName, out enabled);
            return enabled;
        }
    }
}
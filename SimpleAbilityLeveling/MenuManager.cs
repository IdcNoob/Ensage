namespace SimpleAbilityLeveling
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly string heroName;

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager(List<string> abilties, string name)
        {
            heroName = name;

            var advancedMenu = new Menu("Advanced", "advanced");

            foreach (var spell in abilties)
            {
                var abilityMenu = new Menu(string.Empty, spell, textureName: spell);
                abilityMenu.AddItem(
                    new MenuItem(spell + "levelLock", "Lock ability at level").SetValue(
                        new StringList(new[] { "0", "1", "2", "3", "4", "5", "6" })));
                abilityMenu.AddItem(new MenuItem(spell + "fullLock", "Full lock").SetValue(false))
                    .SetTooltip(
                        "If enabled, this ability will be leveled after attributes at level ~23, otherwise before, at level ~12");
                abilityMenu.AddItem(
                    new MenuItem(spell + "forceLearn", "Force learn at hero level").SetValue(
                        new StringList(
                            new[]
                                {
                                    "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                                    "16", "17", "18", "19", "20", "21", "22", "23"
                                })));
                advancedMenu.AddSubMenu(abilityMenu);
            }

            abilties.Reverse(); // correct initial order for PriorityChanger

            menu = new Menu("Ability Leveling", "simpleAbilityLeveling", true);
            menu.AddItem(new MenuItem("enabled", "Enabled for current hero", true).SetValue(false));
            menu.AddItem(
                new MenuItem("priority", "Priority", true).SetValue(
                    new PriorityChanger(abilties, heroName + "priorityChanger", true)));

            menu.AddSubMenu(advancedMenu);
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool IsEnabled => menu.Item(heroName + "enabled").IsActive();

        #endregion

        #region Public Methods and Operators

        public bool AbilityActive(string name)
        {
            return menu.Item(heroName + "priority").GetValue<PriorityChanger>().AbilityToggler.IsEnabled(name);
        }

        public bool AbilityFullLocked(string abilityName)
        {
            return menu.Item(abilityName + "fullLock").IsActive();
        }

        public int AbilityLockLevel(string abilityName)
        {
            return menu.Item(abilityName + "levelLock").GetValue<StringList>().SelectedIndex;
        }

        public int ForceAbilityLearn(string abilityName)
        {
            return menu.Item(abilityName + "forceLearn").GetValue<StringList>().SelectedIndex;
        }

        public uint GetAbilityPriority(string name)
        {
            return menu.Item(heroName + "priority").GetValue<PriorityChanger>().GetPriority(name);
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}
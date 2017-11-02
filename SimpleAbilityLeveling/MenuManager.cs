namespace SimpleAbilityLeveling
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage.Common.Menu;

    internal class MenuManager
    {
        private readonly Dictionary<string, MenuItem> abilityItems = new Dictionary<string, MenuItem>();

        private readonly MenuItem abilityPriority;

        private readonly MenuItem abilityToggler;

        private readonly MenuItem enabledAuto;

        private readonly MenuItem enabledManual;

        private readonly MenuItem heroLevel;

        private readonly Menu menu;

        private readonly MenuItem showAutoBuild;

        public MenuManager(List<string> abilties, string name)
        {
            var advancedMenu = new Menu("Advanced", "advanced");

            var levels = new string[26];
            for (var i = 0; i < levels.Length; i++)
            {
                levels[i] = i.ToString();
            }

            foreach (var spell in abilties)
            {
                var abilityMenu = new Menu(string.Empty, spell, textureName: spell);

                var key = spell + "levelLock";
                var abilityLocked =
                    new MenuItem(key, "Lock ability at level").SetValue(new StringList(new[] { "0", "1", "2", "3", "4", "5", "6" }));
                abilityMenu.AddItem(abilityLocked);
                abilityItems.Add(key, abilityLocked);

                key = spell + "fullLock";
                var abilityFullLocked = new MenuItem(key, "Full lock").SetValue(false);
                abilityMenu.AddItem(abilityFullLocked)
                    .SetTooltip("If enabled, this ability will be leveled after attributes at level ~23, otherwise before, at level ~12");
                abilityItems.Add(key, abilityFullLocked);

                key = spell + "forceLearn";
                var forceLearn = new MenuItem(key, "Force learn at hero level").SetValue(new StringList(levels));
                abilityMenu.AddItem(forceLearn);
                abilityItems.Add(key, forceLearn);

                advancedMenu.AddSubMenu(abilityMenu);
            }

            abilties.Reverse(); // correct initial order for PriorityChanger

            menu = new Menu("Ability Leveling", "simpleAbilityLeveling", true, "attribute_bonus", true);
            menu.AddItem(
                    enabledAuto = new MenuItem("enabledAuto", "Enabled auto mode", true).SetValue(false)
                        .SetTooltip("Abilities will be leveled by biggest win rate build on dotabuff.com (all settings will be ignored)"))
                .ValueChanged += (sender, args) =>
                {
                    if (args.GetNewValue<bool>())
                    {
                        enabledManual.SetValue(false);
                    }
                };

            menu.AddItem(
                    enabledManual = new MenuItem("enabledManual", "Enabled manual mode", true).SetValue(false)
                        .SetTooltip("Abilties will be leveled by selected order and settings"))
                .ValueChanged += (sender, args) =>
                {
                    if (args.GetNewValue<bool>())
                    {
                        enabledAuto.SetValue(false);
                    }
                };

            menu.AddItem(showAutoBuild = new MenuItem("showAutoBuild", "Show auto build preview", true).SetValue(true));

            menu.AddItem(
                heroLevel = new MenuItem("heroLevel", "Required hero level", true).SetValue(new StringList(levels.Skip(1).ToArray()))
                    .SetTooltip("Will start leveling abilities only when your hero will reach selected level"));

            menu.AddItem(
                abilityToggler =
                    new MenuItem(name + "togglerFix", "Enabled", true).SetValue(
                        new AbilityToggler(abilties.ToDictionary(x => x, x => true))));

            menu.AddItem(abilityPriority = new MenuItem(name + "priorityFix", "Priority", true).SetValue(new PriorityChanger(abilties)));

            menu.AddSubMenu(advancedMenu);
            menu.AddToMainMenu();
        }

        public bool IsEnabledAuto => enabledAuto.IsActive();

        public bool IsEnabledManual => enabledManual.IsActive();

        public bool IsOpen => menu.IsOpen;

        public bool ShowAutoBuild => showAutoBuild.IsActive();

        public bool AbilityActive(string abilityName)
        {
            return abilityToggler.GetValue<AbilityToggler>().IsEnabled(abilityName);
        }

        public bool AbilityFullyLocked(string abilityName)
        {
            MenuItem item;
            return abilityItems.TryGetValue(abilityName + "fullLock", out item) && item.IsActive();
        }

        public int AbilityLockLevel(string abilityName)
        {
            MenuItem item;
            return abilityItems.TryGetValue(abilityName + "levelLock", out item) ? item.GetValue<StringList>().SelectedIndex : 0;
        }

        public int ForceAbilityLearnHeroLevel(string abilityName)
        {
            MenuItem item;
            return abilityItems.TryGetValue(abilityName + "forceLearn", out item) ? item.GetValue<StringList>().SelectedIndex : 0;
        }

        public uint GetAbilityPriority(string abilityName)
        {
            return abilityPriority.GetValue<PriorityChanger>().GetPriority(abilityName);
        }

        public bool IsLevelIgnored(uint level)
        {
            return heroLevel.GetValue<StringList>().SelectedIndex + 1 > level;
        }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }
    }
}
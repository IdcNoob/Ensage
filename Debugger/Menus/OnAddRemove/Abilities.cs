namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class Abilities
    {
        public Abilities(Menu mainMenu)
        {
            var menu = new Menu("Abilities ", "abilities");

            var onAddEnabled = new MenuItem("onAbilityAddEnabled", "On add enabled").SetValue(false)
                .SetTooltip("ObjectManager.OnAddEntity");
            menu.AddItem(onAddEnabled);
            onAddEnabled.ValueChanged += (sender, args) =>
                {
                    OnAddEnabled = args.GetNewValue<bool>();
                    if (OnAddEnabled && !menu.DisplayName.EndsWith("*"))
                    {
                        menu.DisplayName = menu.DisplayName += "*";
                    }
                    else if (!OnRemoveEnabled)
                    {
                        menu.DisplayName = menu.DisplayName.TrimEnd('*');
                    }
                };
            OnAddEnabled = onAddEnabled.IsActive();
            if (OnAddEnabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var onRemoveEnabled = new MenuItem("onAbilityRemoveEnabled", "On remove enabled").SetValue(false)
                .SetTooltip("ObjectManager.OnRemoveEntity");
            menu.AddItem(onRemoveEnabled);
            onRemoveEnabled.ValueChanged += (sender, args) =>
                {
                    OnRemoveEnabled = args.GetNewValue<bool>();
                    if (OnRemoveEnabled && !menu.DisplayName.EndsWith("*"))
                    {
                        menu.DisplayName = menu.DisplayName += "*";
                    }
                    else if (!OnAddEnabled)
                    {
                        menu.DisplayName = menu.DisplayName.TrimEnd('*');
                    }
                };
            OnRemoveEnabled = onRemoveEnabled.IsActive();
            if (OnRemoveEnabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var heroesOnly = new MenuItem("abilitiesHeroesOnly", "Hero abilities only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            var ignoreUnits = new MenuItem("ignoreAbilities", "Ignore useless abilities").SetValue(true);
            menu.AddItem(ignoreUnits);
            ignoreUnits.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreUnits.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool HeroesOnly { get; private set; }

        public bool IgnoreUseless { get; private set; }

        public bool OnAddEnabled { get; private set; }

        public bool OnRemoveEnabled { get; private set; }
    }
}
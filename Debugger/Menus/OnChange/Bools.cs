namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Bools
    {
        public Bools(Menu mainMenu)
        {
            var menu = new Menu("Bools ", "bools");

            var enabled = new MenuItem("boolsEnabled", "Enabled").SetValue(false)
                .SetTooltip("Entity.OnBoolPropertyChange");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    Enabled = args.GetNewValue<bool>();
                    if (Enabled)
                    {
                        menu.DisplayName = menu.DisplayName += "*";
                    }
                    else
                    {
                        menu.DisplayName = menu.DisplayName.TrimEnd('*');
                    }
                };
            Enabled = enabled.IsActive();
            if (Enabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var heroesOnly = new MenuItem("boolsHeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            var ignoreUseless = new MenuItem("ignoreBools", "Ignore useless bools").SetValue(false);
            menu.AddItem(ignoreUseless);
            ignoreUseless.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreUseless.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool Enabled { get; private set; }

        public bool IgnoreUseless { get; private set; }

        public bool HeroesOnly { get; private set; }
    }
}
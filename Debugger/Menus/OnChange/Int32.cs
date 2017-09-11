namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Int32
    {
        public Int32(Menu mainMenu)
        {
            var menu = new Menu("Int32 ", "int32");

            var enabled = new MenuItem("int32Enabled", "Enabled").SetValue(false)
                .SetTooltip("Entity.OnInt32PropertyChange");
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

            var heroesOnly = new MenuItem("int32HeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            var ignoreUseless = new MenuItem("ignoreInt32", "Ignore useless ints").SetValue(false);
            menu.AddItem(ignoreUseless);
            ignoreUseless.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreUseless.IsActive();

            var ignoreSemiUseless = new MenuItem("semiIgnoreInt32", "Ignore semi useless ints").SetValue(false);
            menu.AddItem(ignoreSemiUseless);
            ignoreSemiUseless.ValueChanged += (sender, args) => IgnoreSemiUseless = args.GetNewValue<bool>();
            IgnoreSemiUseless = ignoreSemiUseless.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool Enabled { get; private set; }

        public bool HeroesOnly { get; private set; }

        public bool IgnoreSemiUseless { get; private set; }

        public bool IgnoreUseless { get; private set; }
    }
}
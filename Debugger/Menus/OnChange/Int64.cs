namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Int64
    {
        public Int64(Menu mainMenu)
        {
            var menu = new Menu("Int64 ", "int64");

            var enabled = new MenuItem("int64Enabled", "Enabled").SetValue(false)
                .SetTooltip("Entity.OnInt64PropertyChange");
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

            var heroesOnly = new MenuItem("int64HeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool Enabled { get; private set; }

        public bool HeroesOnly { get; private set; }
    }
}
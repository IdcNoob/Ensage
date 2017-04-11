namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Handles
    {
        public Handles(Menu mainMenu)
        {
            var menu = new Menu("Handles ", "handles");

            var enabled = new MenuItem("handlesEnabled", "Enabled").SetValue(false)
                .SetTooltip("Entity.OnHandlePropertyChange");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => Enabled = args.GetNewValue<bool>();
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

            var heroesOnly = new MenuItem("handlesHeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool Enabled { get; private set; }

        public bool HeroesOnly { get; private set; }
    }
}
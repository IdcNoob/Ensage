namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Animations
    {
        public Animations(Menu mainMenu)
        {
            var menu = new Menu("Animations ", "animations");

            var enabled = new MenuItem("animationsEnabled", "Enabled").SetValue(false)
                .SetTooltip("Entity.OnAnimationChanged");
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

            var heroesOnly = new MenuItem("animationsHeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool Enabled { get; private set; }

        public bool HeroesOnly { get; private set; }
    }
}
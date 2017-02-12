namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Strings
    {
        #region Constructors and Destructors

        public Strings(Menu mainMenu)
        {
            var menu = new Menu("Strings ", "strings");

            var enabled =
                new MenuItem("stringsEnabled", "Enabled").SetValue(false).SetTooltip("Entity.OnStringPropertyChange");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => Enabled = args.GetNewValue<bool>();
            enabled.ValueChanged += (sender, args) => {
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

            var heroesOnly = new MenuItem("stringsHeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool Enabled { get; private set; }

        public bool HeroesOnly { get; private set; }

        #endregion
    }
}
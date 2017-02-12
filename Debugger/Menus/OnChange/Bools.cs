namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Bools
    {
        #region Constructors and Destructors

        public Bools(Menu mainMenu)
        {
            var menu = new Menu("Bools ", "bools");

            var enabled =
                new MenuItem("boolsEnabled", "Enabled").SetValue(false).SetTooltip("Entity.OnBoolPropertyChange");
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

            var heroesOnly = new MenuItem("boolsHeroesOnly", "Heroes only").SetValue(false);
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
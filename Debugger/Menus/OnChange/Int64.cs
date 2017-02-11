namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Int64
    {
        #region Constructors and Destructors

        public Int64(Menu mainMenu)
        {
            var menu = new Menu("Int64", "int64");

            var enabled = new MenuItem("int64Enabled", "Enabled").SetValue(false);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => Enabled = args.GetNewValue<bool>();
            Enabled = enabled.IsActive();

            var heroesOnly = new MenuItem("int64HeroesOnly", "Heroes only").SetValue(false);
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
namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Floats
    {
        #region Constructors and Destructors

        public Floats(Menu mainMenu)
        {
            var menu = new Menu("Floats", "floats");

            var enabled = new MenuItem("floatsEnabled", "Enabled").SetValue(false);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => Enabled = args.GetNewValue<bool>();
            Enabled = enabled.IsActive();

            var heroesOnly = new MenuItem("floatsHeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            var ignoreUseless = new MenuItem("ignoreFloats", "Ignore useless floats").SetValue(false);
            menu.AddItem(ignoreUseless);
            ignoreUseless.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreUseless.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool Enabled { get; private set; }

        public bool HeroesOnly { get; private set; }

        public bool IgnoreUseless { get; private set; }

        #endregion
    }
}
namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Int32
    {
        #region Constructors and Destructors

        public Int32(Menu mainMenu)
        {
            var menu = new Menu("Int32", "int32");

            var enabled = new MenuItem("int32Enabled", "Enabled").SetValue(false);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => Enabled = args.GetNewValue<bool>();
            Enabled = enabled.IsActive();

            var heroesOnly = new MenuItem("int32HeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            var ignoreUseless = new MenuItem("ignoreInt32", "Ignore useless ints").SetValue(false);
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
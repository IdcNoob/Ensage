namespace Debugger.Menus.OnChange
{
    using Ensage.Common.Menu;

    internal class Handles
    {
        #region Constructors and Destructors

        public Handles(Menu mainMenu)
        {
            var menu = new Menu("Handles", "handles");

            var enabled = new MenuItem("handlesEnabled", "Enabled").SetValue(false);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => Enabled = args.GetNewValue<bool>();
            Enabled = enabled.IsActive();

            var heroesOnly = new MenuItem("handlesHeroesOnly", "Heroes only").SetValue(false);
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
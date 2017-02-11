namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class Modifiers
    {
        #region Constructors and Destructors

        public Modifiers(Menu mainMenu)
        {
            var menu = new Menu("Modifiers", "modifiers");

            var onAddEnabled = new MenuItem("onModifierAddEnabled", "On add enabled").SetValue(false);
            menu.AddItem(onAddEnabled);
            onAddEnabled.ValueChanged += (sender, args) => OnAddEnabled = args.GetNewValue<bool>();
            OnAddEnabled = onAddEnabled.IsActive();

            var onRemoveEnabled = new MenuItem("onModifierRemoveEnabled", "On remove enabled").SetValue(false);
            menu.AddItem(onRemoveEnabled);
            onRemoveEnabled.ValueChanged += (sender, args) => OnRemoveEnabled = args.GetNewValue<bool>();
            OnRemoveEnabled = onRemoveEnabled.IsActive();

            var heroesOnly = new MenuItem("modifierHeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            var ignoreModifiers = new MenuItem("ignoreModifiers", "Ignore useless modifiers").SetValue(false);
            menu.AddItem(ignoreModifiers);
            ignoreModifiers.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreModifiers.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool HeroesOnly { get; private set; }

        public bool IgnoreUseless { get; private set; }

        public bool OnAddEnabled { get; private set; }

        public bool OnRemoveEnabled { get; private set; }

        #endregion
    }
}
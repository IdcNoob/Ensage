namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class Abilities
    {
        #region Constructors and Destructors

        public Abilities(Menu mainMenu)
        {
            var menu = new Menu("Abilities", "abilities");

            var onAddEnabled = new MenuItem("onAbilityAddEnabled", "On add enabled").SetValue(false);
            menu.AddItem(onAddEnabled);
            onAddEnabled.ValueChanged += (sender, args) => OnAddEnabled = args.GetNewValue<bool>();
            OnAddEnabled = onAddEnabled.IsActive();

            var onRemoveEnabled = new MenuItem("onAbilityRemoveEnabled", "On remove enabled").SetValue(false);
            menu.AddItem(onRemoveEnabled);
            onRemoveEnabled.ValueChanged += (sender, args) => OnRemoveEnabled = args.GetNewValue<bool>();
            OnRemoveEnabled = onRemoveEnabled.IsActive();

            var heroesOnly = new MenuItem("abilitiesHeroesOnly", "Hero abilities only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            var ignoreUnits = new MenuItem("ignoreAbilities", "Ignore useless abilities").SetValue(true);
            menu.AddItem(ignoreUnits);
            ignoreUnits.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreUnits.IsActive();

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
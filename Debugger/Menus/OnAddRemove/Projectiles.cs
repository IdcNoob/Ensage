namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class Projectiles
    {
        #region Constructors and Destructors

        public Projectiles(Menu mainMenu)
        {
            var menu = new Menu("Projectiles ", "projectiles");

            var onAddEnabled =
                new MenuItem("onProjectileAddEnabled", "On add enabled").SetValue(false)
                    .SetTooltip("ObjectManager.OnAddTrackingProjectile");
            menu.AddItem(onAddEnabled);
            onAddEnabled.ValueChanged += (sender, args) => {
                OnAddEnabled = args.GetNewValue<bool>();
                if (OnAddEnabled && !menu.DisplayName.EndsWith("*"))
                {
                    menu.DisplayName = menu.DisplayName += "*";
                }
                else if (!OnRemoveEnabled)
                {
                    menu.DisplayName = menu.DisplayName.TrimEnd('*');
                }
            };
            OnAddEnabled = onAddEnabled.IsActive();
            if (OnAddEnabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var onRemoveEnabled =
                new MenuItem("onProjectileRemoveEnabled", "On remove enabled").SetValue(false)
                    .SetTooltip("ObjectManager.OnRemoveTrackingProjectile");
            menu.AddItem(onRemoveEnabled);
            onRemoveEnabled.ValueChanged += (sender, args) => {
                OnRemoveEnabled = args.GetNewValue<bool>();
                if (OnRemoveEnabled && !menu.DisplayName.EndsWith("*"))
                {
                    menu.DisplayName = menu.DisplayName += "*";
                }
                else if (!OnAddEnabled)
                {
                    menu.DisplayName = menu.DisplayName.TrimEnd('*');
                }
            };
            OnRemoveEnabled = onRemoveEnabled.IsActive();
            if (OnRemoveEnabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var heroesOnly = new MenuItem("projectileHeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool HeroesOnly { get; private set; }

        public bool OnAddEnabled { get; private set; }

        public bool OnRemoveEnabled { get; private set; }

        #endregion
    }
}
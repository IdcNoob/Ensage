namespace Debugger.Menus.OnAddRemove
{
    using Ensage.Common.Menu;

    internal class Particles
    {
        #region Constructors and Destructors

        public Particles(Menu mainMenu)
        {
            var menu = new Menu("Particles ", "particles");

            var onAddEnabled =
                new MenuItem("onParticleAddEnabled", "On add enabled").SetValue(false)
                    .SetTooltip("Entity.OnParticleEffectAdded");
            menu.AddItem(onAddEnabled);
            onAddEnabled.ValueChanged += (sender, args) => {
                OnAddEnabled = args.GetNewValue<bool>();
                if (OnAddEnabled)
                {
                    menu.DisplayName = menu.DisplayName += "*";
                }
                else
                {
                    menu.DisplayName = menu.DisplayName.TrimEnd('*');
                }
            };
            OnAddEnabled = onAddEnabled.IsActive();
            if (OnAddEnabled)
            {
                menu.DisplayName = menu.DisplayName += "*";
            }

            var heroesOnly = new MenuItem("partcileHeroesOnly", "Heroes only").SetValue(false);
            menu.AddItem(heroesOnly);
            heroesOnly.ValueChanged += (sender, args) => HeroesOnly = args.GetNewValue<bool>();
            HeroesOnly = heroesOnly.IsActive();

            var cpValues = new MenuItem("partcileCpValues", "Show cp values").SetValue(false);
            menu.AddItem(cpValues);
            cpValues.ValueChanged += (sender, args) => ShowControlPointValues = args.GetNewValue<bool>();
            ShowControlPointValues = cpValues.IsActive();

            var ignoreCpValues = new MenuItem("partcileIgnoreCpValues", "Ignore zero cp values").SetValue(false);
            menu.AddItem(ignoreCpValues);
            ignoreCpValues.ValueChanged += (sender, args) => IgnoreZeroControlPointValues = args.GetNewValue<bool>();
            IgnoreZeroControlPointValues = ignoreCpValues.IsActive();

            var ignoreModifiers = new MenuItem("ignoreParticles", "Ignore useless particles").SetValue(false);
            menu.AddItem(ignoreModifiers);
            ignoreModifiers.ValueChanged += (sender, args) => IgnoreUseless = args.GetNewValue<bool>();
            IgnoreUseless = ignoreModifiers.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool HeroesOnly { get; private set; }

        public bool IgnoreUseless { get; private set; }

        public bool IgnoreZeroControlPointValues { get; private set; }

        public bool OnAddEnabled { get; private set; }

        public bool ShowControlPointValues { get; private set; }

        #endregion
    }
}
namespace ItemManager.Menus.Modules.AutoActions.HpMpRestore
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class InstantHealthRestoreItemMenu
    {
        public InstantHealthRestoreItemMenu(Menu mainMenu, string name, bool defaultEnabled = true)
        {
            var simpleName = name.ToLower().Replace(" ", string.Empty);
            var menu = new Menu(name, simpleName + "Menu");

            var enabled = new MenuItem(simpleName + "Enabled", "Enabled").SetValue(defaultEnabled);
            enabled.SetTooltip("Auto use item on low health");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var hpThreshold = new MenuItem(simpleName + "Hp", "HP threshold").SetValue(new Slider(150, 50, 500));
            hpThreshold.SetTooltip("Use item if you have less HP");
            menu.AddItem(hpThreshold);
            hpThreshold.ValueChanged += (sender, args) => HealthThreshold = args.GetNewValue<Slider>().Value;
            HealthThreshold = hpThreshold.GetValue<Slider>().Value;

            var hpThresholdPct = new MenuItem(simpleName + "HpPct", "HP% threshold").SetValue(new Slider(20, 1, 50));
            hpThresholdPct.SetTooltip("Use item if you have less HP%");
            menu.AddItem(hpThresholdPct);
            hpThresholdPct.ValueChanged += (sender, args) => HealthThresholdPct = args.GetNewValue<Slider>().Value;
            HealthThresholdPct = hpThresholdPct.GetValue<Slider>().Value;

            var enemyCheckRange = new MenuItem(simpleName + "EnemyRange", "Enemy search range").SetValue(new Slider(800, 0, 2000));
            enemyCheckRange.SetTooltip("Use item only if there is enemy hero in range (if set to 0 range won't be checked)");
            menu.AddItem(enemyCheckRange);
            enemyCheckRange.ValueChanged += (sender, args) => EnemySearchRange = args.GetNewValue<Slider>().Value;
            EnemySearchRange = enemyCheckRange.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public int EnemySearchRange { get; private set; }

        public int HealthThreshold { get; private set; }

        public int HealthThresholdPct { get; private set; }

        public bool IsEnabled { get; private set; }
    }
}
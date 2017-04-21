namespace ItemManager.Menus.Modules.AutoActions.HpMpRestore
{
    using Ensage.Common.Menu;

    internal class AutoMagicStickMenu
    {
        public AutoMagicStickMenu(Menu mainMenu)
        {
            var menu = new Menu("Magic stick", "magicStickMenu");

            var enabled = new MenuItem("magicStickEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use magic stick/wand on low health");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            var hpThreshold = new MenuItem("magicStickHp", "HP threshold").SetValue(new Slider(150, 50, 500));
            hpThreshold.SetTooltip("Use magic stick/wand if you have less HP");
            menu.AddItem(hpThreshold);
            hpThreshold.ValueChanged += (sender, args) => HealthThreshold = args.GetNewValue<Slider>().Value;
            HealthThreshold = hpThreshold.GetValue<Slider>().Value;

            var hpThresholdPct = new MenuItem("magicStickHpPct", "HP% threshold").SetValue(new Slider(20, 1, 50));
            hpThresholdPct.SetTooltip("Use magic stick/wand if you have less HP%");
            menu.AddItem(hpThresholdPct);
            hpThresholdPct.ValueChanged += (sender, args) => HealthThresholdPct = args.GetNewValue<Slider>().Value;
            HealthThresholdPct = hpThresholdPct.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public int HealthThreshold { get; private set; }

        public int HealthThresholdPct { get; private set; }

        public bool IsEnabled { get; private set; }
    }
}
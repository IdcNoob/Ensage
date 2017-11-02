namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class MidasMenu
    {
        public MidasMenu(Menu mainMenu)
        {
            var menu = new Menu("Midas", "midasMenu");

            var enabled = new MenuItem("midasEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use hand of midas");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var healthThresholdPct = new MenuItem("midasHealthPct", "HP% threshold").SetValue(new Slider(70, 0, 100));
            healthThresholdPct.SetTooltip("Use hand of midas only when creep has more hp%");
            menu.AddItem(healthThresholdPct);
            healthThresholdPct.ValueChanged += (sender, args) => HealthThresholdPct = args.GetNewValue<Slider>().Value;
            HealthThresholdPct = healthThresholdPct.GetValue<Slider>().Value;

            var experienceThreshold = new MenuItem("midasExpPct", "Experience threshold").SetValue(new Slider(90, 0, 150));
            experienceThreshold.SetTooltip("Use hand of midas only when creep will grant more exp (base)");
            menu.AddItem(experienceThreshold);
            experienceThreshold.ValueChanged += (sender, args) => ExperienceThreshold = args.GetNewValue<Slider>().Value;
            ExperienceThreshold = experienceThreshold.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public int ExperienceThreshold { get; private set; }

        public int HealthThresholdPct { get; private set; }

        public bool IsEnabled { get; private set; }
    }
}
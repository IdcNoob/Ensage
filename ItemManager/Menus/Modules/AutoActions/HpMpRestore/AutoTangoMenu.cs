namespace ItemManager.Menus.Modules.AutoActions.HpMpRestore
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class AutoTangoMenu
    {
        public AutoTangoMenu(Menu mainMenu)
        {
            var menu = new Menu("Tango", "tangoMenu");

            var enabled = new MenuItem("tangoEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use tango on happy little tree");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            var missingHp = new MenuItem("tangoMissingHp", "Health threshold").SetValue(new Slider(150, 50, 250));
            missingHp.SetTooltip("Use tango only when missing more hp");
            menu.AddItem(missingHp);
            missingHp.ValueChanged += (sender, args) => HealthThreshold = args.GetNewValue<Slider>().Value;
            HealthThreshold = missingHp.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public int HealthThreshold { get; private set; }

        public bool IsEnabled { get; private set; }
    }
}
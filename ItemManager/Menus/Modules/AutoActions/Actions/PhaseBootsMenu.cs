namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using Ensage.Common.Menu;

    internal class PhaseBootsMenu
    {
        public PhaseBootsMenu(Menu mainMenu)
        {
            var menu = new Menu("Phase boots", "phaseBootsMenu");

            var enabled = new MenuItem("phaseBootsEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use phase boots");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsEnabled = args.GetNewValue<bool>();
            IsEnabled = enabled.IsActive();

            var damageThreshold =
                new MenuItem("phaseBootsDistance", "Distance check").SetValue(new Slider(700, 0, 2000));
            damageThreshold.SetTooltip("Use phase boots only when move distance is bigger");
            menu.AddItem(damageThreshold);
            damageThreshold.ValueChanged += (sender, args) => Distance = args.GetNewValue<Slider>().Value;
            Distance = damageThreshold.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public int Distance { get; private set; }

        public bool IsEnabled { get; private set; }
    }
}
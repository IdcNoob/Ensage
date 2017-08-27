namespace ItemManager.Menus.Modules.OffensiveAbilities.AbilitySettings
{
    using Ensage.Common.Menu;

    internal class SatanicSettings : OffensiveAbilitySettings
    {
        public SatanicSettings(Menu mainMenu, string name, string texture = null)
            : base(mainMenu, name, texture)
        {
            var healthThreshold = new MenuItem("satancHealth", "Health threshold%").SetValue(new Slider(20, 1, 99));
            healthThreshold.SetTooltip("Use ability when health% is lower");
            Menu.AddItem(healthThreshold);
            healthThreshold.ValueChanged += (sender, args) => HealthThreshold = args.GetNewValue<Slider>().Value;
            HealthThreshold = healthThreshold.GetValue<Slider>().Value;
        }

        public int HealthThreshold { get; protected set; }
    }
}
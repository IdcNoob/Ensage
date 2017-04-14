namespace ItemManager.Menus.Modules.Recovery.ItemSettings
{
    using Ensage.Common.Menu;

    internal class SoulRingSettings : ItemSettingsMenu
    {
        public SoulRingSettings(Menu mainMenu, string name, int? initialHp = null, int? initialMp = null)
            : base(mainMenu, name, initialHp, initialMp)
        {
            var hpThreshold = new MenuItem("hpThresholdSoulRing", "HP% threshold").SetValue(new Slider(60, 1, 100));
            hpThreshold.SetTooltip("Use only when you have more hp%");
            Menu.AddItem(hpThreshold);
            hpThreshold.ValueChanged += (sender, args) => HpThreshold = args.GetNewValue<Slider>().Value;
            HpThreshold = hpThreshold.GetValue<Slider>().Value;
        }
    }
}
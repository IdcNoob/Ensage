namespace ItemManager.Menus.Modules.ShrineHelper
{
    using Ensage.Common.Menu;

    internal class ShrineHelperMenu
    {
        public ShrineHelperMenu(Menu mainMenu)
        {
            var menu = new Menu("Shrine helper", "shrineHelper");

            var enabled = new MenuItem("shrineBlock", "Block shrine usage").SetValue(true);
            enabled.SetTooltip("Block accidental shrine usage");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => BlockShrineUsage = args.GetNewValue<bool>();
            BlockShrineUsage = enabled.IsActive();

            var hpThreshold = new MenuItem("hpThreshold", "HP% threshold").SetValue(new Slider(85, 50, 99));
            hpThreshold.SetTooltip("Block shrine usage if you have more hp%");
            menu.AddItem(hpThreshold);
            hpThreshold.ValueChanged += (sender, args) => HpThreshold = args.GetNewValue<Slider>().Value;
            HpThreshold = hpThreshold.GetValue<Slider>().Value;

            var hpThresholdPct = new MenuItem("mpThreshold", "MP% threshold").SetValue(new Slider(85, 50, 99));
            hpThresholdPct.SetTooltip("Block shrine usage if you have more mp%");
            menu.AddItem(hpThresholdPct);
            hpThresholdPct.ValueChanged += (sender, args) => MpThreshold = args.GetNewValue<Slider>().Value;
            MpThreshold = hpThresholdPct.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public bool BlockShrineUsage { get; private set; }

        public int HpThreshold { get; private set; }

        public int MpThreshold { get; private set; }
    }
}
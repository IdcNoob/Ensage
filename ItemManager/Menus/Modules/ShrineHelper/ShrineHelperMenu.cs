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

            var hpThreshold = new MenuItem("shrineHpThreshold", "HP% threshold").SetValue(new Slider(85, 50, 99));
            hpThreshold.SetTooltip("Block shrine usage if you have more hp%");
            menu.AddItem(hpThreshold);
            hpThreshold.ValueChanged += (sender, args) => HpThreshold = args.GetNewValue<Slider>().Value;
            HpThreshold = hpThreshold.GetValue<Slider>().Value;

            var mpThreshold = new MenuItem("shrineMpThreshold", "MP% threshold").SetValue(new Slider(85, 50, 99));
            mpThreshold.SetTooltip("Block shrine usage if you have more mp%");
            menu.AddItem(mpThreshold);
            mpThreshold.ValueChanged += (sender, args) => MpThreshold = args.GetNewValue<Slider>().Value;
            MpThreshold = mpThreshold.GetValue<Slider>().Value;

            var autoDisable = new MenuItem("shrineAutoDisableItems", "Auto disable items").SetValue(false)
                .SetTooltip("Auto \"disable\" items when using shrine and there is no enemies near");
            menu.AddItem(autoDisable);
            autoDisable.ValueChanged += (sender, args) => AutoDisableItems = args.GetNewValue<bool>();
            AutoDisableItems = autoDisable.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool AutoDisableItems { get; private set; }

        public bool BlockShrineUsage { get; private set; }

        public int HpThreshold { get; private set; }

        public int MpThreshold { get; private set; }
    }
}
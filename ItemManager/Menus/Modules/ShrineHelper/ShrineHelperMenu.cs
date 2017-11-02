namespace ItemManager.Menus.Modules.ShrineHelper
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class ShrineHelperMenu
    {
        public ShrineHelperMenu(Menu mainMenu)
        {
            var menu = new Menu("Shrine helper", "shrineHelper");

            var enabled = new MenuItem("shrineBlock", "Block shrine usage").SetValue(true);
            enabled.SetTooltip("Block accidental shrine usage");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    BlockShrineUsage = args.GetNewValue<bool>();
                    OnBlockEnabledChange?.Invoke(null, new BoolEventArgs(BlockShrineUsage));
                };
            BlockShrineUsage = enabled.IsActive();

            var hpUseThreshold = new MenuItem("shrineHpThreshold", "HP% threshold (use)").SetValue(new Slider(85, 50, 99));
            hpUseThreshold.SetTooltip("Block shrine usage if you have more hp%");
            menu.AddItem(hpUseThreshold);
            hpUseThreshold.ValueChanged += (sender, args) => HpUseThreshold = args.GetNewValue<Slider>().Value;
            HpUseThreshold = hpUseThreshold.GetValue<Slider>().Value;

            var mpUseThreshold = new MenuItem("shrineMpThreshold", "MP% threshold (use)").SetValue(new Slider(85, 50, 99));
            mpUseThreshold.SetTooltip("Block shrine usage if you have more mp%");
            menu.AddItem(mpUseThreshold);
            mpUseThreshold.ValueChanged += (sender, args) => MpUseThreshold = args.GetNewValue<Slider>().Value;
            MpUseThreshold = mpUseThreshold.GetValue<Slider>().Value;

            var autoDisable = new MenuItem("shrineAutoDisableItems", "Auto disable items").SetValue(false)
                .SetTooltip("Auto \"disable\" items when using shrine and there is no enemies near");
            menu.AddItem(autoDisable);
            autoDisable.ValueChanged += (sender, args) =>
                {
                    AutoDisableItems = args.GetNewValue<bool>();
                    OnDisableItemsChange?.Invoke(null, new BoolEventArgs(AutoDisableItems));
                };
            AutoDisableItems = autoDisable.IsActive();

            var hpDisableThreshold = new MenuItem("shrineHpThresholdDisable", "HP% threshold (items)").SetValue(new Slider(80, 50, 99));
            hpDisableThreshold.SetTooltip("Disable items only if you have less hp%");
            menu.AddItem(hpDisableThreshold);
            hpDisableThreshold.ValueChanged += (sender, args) => HpDisableThreshold = args.GetNewValue<Slider>().Value;
            HpDisableThreshold = hpDisableThreshold.GetValue<Slider>().Value;

            var mpDisableThreshold = new MenuItem("shrineMpThresholdDisable", "MP% threshold (items)").SetValue(new Slider(80, 50, 99));
            mpDisableThreshold.SetTooltip("Disable items only if you have less mp%");
            menu.AddItem(mpDisableThreshold);
            mpDisableThreshold.ValueChanged += (sender, args) => MpDisableThreshold = args.GetNewValue<Slider>().Value;
            MpDisableThreshold = mpDisableThreshold.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnBlockEnabledChange;

        public event EventHandler<BoolEventArgs> OnDisableItemsChange;

        public bool AutoDisableItems { get; private set; }

        public bool BlockShrineUsage { get; private set; }

        public int HpDisableThreshold { get; private set; }

        public int HpUseThreshold { get; private set; }

        public int MpDisableThreshold { get; private set; }

        public int MpUseThreshold { get; private set; }
    }
}
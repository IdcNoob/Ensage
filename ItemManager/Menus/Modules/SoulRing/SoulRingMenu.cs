namespace ItemManager.Menus.Modules.SoulRing
{
    using Ensage.Common.Menu;

    internal class SoulRingMenu
    {
        public SoulRingMenu(Menu mainMenu)
        {
            var menu = new Menu("Auto soul ring", "soulRing");

            var enabled = new MenuItem("autoSoulRing", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto soul ring when using abilities");
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) => IsActive = args.GetNewValue<bool>();
            IsActive = enabled.IsActive();

            var hpThreshold = new MenuItem("hpThreshold", "HP% threshold").SetValue(new Slider(70));
            hpThreshold.SetTooltip("Use soul ring if you have more hp%");
            menu.AddItem(hpThreshold);
            hpThreshold.ValueChanged += (sender, args) => HpThreshold = args.GetNewValue<Slider>().Value;
            HpThreshold = hpThreshold.GetValue<Slider>().Value;

            var mpThreshold = new MenuItem("mpThreshold", "MP% threshold").SetValue(new Slider(100));
            mpThreshold.SetTooltip("Use soul ring if you have less mp%");
            menu.AddItem(mpThreshold);
            mpThreshold.ValueChanged += (sender, args) => MpThreshold = args.GetNewValue<Slider>().Value;
            MpThreshold = mpThreshold.GetValue<Slider>().Value;

            mainMenu.AddSubMenu(menu);
        }

        public int HpThreshold { get; private set; }

        public bool IsActive { get; private set; }

        public int MpThreshold { get; private set; }
    }
}
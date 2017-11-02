namespace ItemManager.Menus.Modules.Recovery.ItemSettings
{
    using Ensage.Common.Menu;

    internal class ItemSettingsMenu
    {
        protected Menu Menu;

        public ItemSettingsMenu(Menu mainMenu, string name, int? initialHp = null, int? initialMp = null)
        {
            var simpleName = name.ToLower().Replace(" ", string.Empty);
            Menu = new Menu(name, simpleName + "Settings");

            if (initialMp != null)
            {
                var mpThreshold =
                    new MenuItem("mpThreshold" + simpleName, "MP threshold").SetValue(new Slider(initialMp.Value, 1, initialMp.Value * 2));
                mpThreshold.SetTooltip("Use only when you are missing more mp");
                Menu.AddItem(mpThreshold);
                mpThreshold.ValueChanged += (sender, args) => MpThreshold = args.GetNewValue<Slider>().Value;
                MpThreshold = mpThreshold.GetValue<Slider>().Value;
            }

            if (initialHp != null)
            {
                var hpThreshold =
                    new MenuItem("hpThreshold" + simpleName, "HP threshold").SetValue(new Slider(initialHp.Value, 1, initialHp.Value * 2));
                hpThreshold.SetTooltip("Use only when you are missing more hp");
                Menu.AddItem(hpThreshold);
                hpThreshold.ValueChanged += (sender, args) => HpThreshold = args.GetNewValue<Slider>().Value;
                HpThreshold = hpThreshold.GetValue<Slider>().Value;
            }

            mainMenu.AddSubMenu(Menu);
        }

        public int HpThreshold { get; protected set; }

        public int MpThreshold { get; protected set; }
    }
}
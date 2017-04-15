namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using Ensage.Common.Menu;

    internal class AutoBottleMenu
    {
        public AutoBottleMenu(Menu mainMenu)
        {
            var menu = new Menu("AutoBottleMenu", "bottle");

            var autoSelfBottle = new MenuItem("autoBottleSelf", "Self bottle").SetValue(true);
            autoSelfBottle.SetTooltip("Auto bottle usage on self while at base");
            menu.AddItem(autoSelfBottle);
            autoSelfBottle.ValueChanged += (sender, args) => AutoSelfBottle = args.GetNewValue<bool>();
            AutoSelfBottle = autoSelfBottle.IsActive();

            var autoAllyBottle = new MenuItem("autoBottleAlly", "Ally bottle").SetValue(true);
            autoAllyBottle.SetTooltip("Auto bottle usage on allies while at base");
            menu.AddItem(autoAllyBottle);
            autoAllyBottle.ValueChanged += (sender, args) => AutoAllyBottle = args.GetNewValue<bool>();
            AutoAllyBottle = autoAllyBottle.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public bool AutoAllyBottle { get; private set; }

        public bool AutoSelfBottle { get; private set; }
    }
}
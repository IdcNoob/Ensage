namespace ItemManager.Menus.Modules.AutoActions
{
    using Ensage.Common.Menu;

    using Heals;

    internal class AutoHealsMenu
    {
        public AutoHealsMenu(Menu rootMenu)
        {
            var menu = new Menu("Heals", "autoHealUsage");

            LivingArmorMenu = new LivingArmorMenu(menu);

            rootMenu.AddSubMenu(menu);
        }

        public LivingArmorMenu LivingArmorMenu { get; }
    }
}
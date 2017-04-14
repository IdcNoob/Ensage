namespace ItemManager.Menus.Modules.AutoUsage
{
    using Ensage.Common.Menu;

    internal class AutoUsageMenu
    {
        public AutoUsageMenu(Menu mainMenu)
        {
            var menu = new Menu("Auto usage", "autoUsage");

            SoulRing = new SoulRing(menu);
            PowerTreads = new PowerTreads(menu);
            Bottle = new Bottle(menu);
            ArcaneBoots = new ArcaneBoots(menu);
            Deward = new Deward(menu);

            mainMenu.AddSubMenu(menu);
        }

        public ArcaneBoots ArcaneBoots { get; }

        public Bottle Bottle { get; }

        public Deward Deward { get; }

        public PowerTreads PowerTreads { get; }

        public SoulRing SoulRing { get; }
    }
}
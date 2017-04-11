namespace ItemManager.Core
{
    using Ensage;

    using Menus;

    using Modules.CourierHelper;
    using Modules.GoldSpender;
    using Modules.HpMpAbuse;
    using Modules.ItemSwapper;
    using Modules.ShrineHelper;
    using Modules.Snatcher;

    internal class Bootstrap
    {
        private CourierHelper courierHelper;

        private GoldSpender goldSpender;

        private HpMpAbuse HpMpAbuse;

        private ItemManager itemManager;

        private ItemSwapper itemSwapper;

        private MenuManager menu;

        private ShrineHelper shrineHelper;

        private Snatcher snatcher;

        public void OnClose()
        {
            menu.OnClose();
            itemSwapper.OnClose();
            courierHelper.OnClose();
            snatcher.OnClose();
            goldSpender.OnClose();
            shrineHelper.OnClose();
            itemManager.OnClose();
            HpMpAbuse.OnClose();
        }

        public void OnLoad()
        {
            var hero = ObjectManager.LocalHero;

            menu = new MenuManager();
            itemManager = new ItemManager(hero, menu);
            itemSwapper = new ItemSwapper(hero, itemManager, menu.ItemSwapMenu);
            courierHelper = new CourierHelper(hero, itemManager, menu.CourierHelperMenu);
            snatcher = new Snatcher(hero, itemManager, menu.SnatcherMenu);
            goldSpender = new GoldSpender(hero, itemManager, menu.GoldSpenderMenu);
            shrineHelper = new ShrineHelper(hero, menu.ShrineHelperMenu);
            HpMpAbuse = new HpMpAbuse(hero, menu);
        }
    }
}
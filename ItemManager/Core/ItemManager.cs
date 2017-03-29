namespace ItemManager.Core
{
    using Ensage;

    using Menus;

    internal class ItemManager
    {
        #region Fields

        private Hero hero;

        private Items items;

        private ItemSwapper itemSwapper;

        //private CourierHelper courierHelper;

        private MenuManager menu;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.OnClose();
            items.OnClose();
            itemSwapper.OnClose();
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            menu = new MenuManager();
            items = new Items(hero, menu.ItemSwapMenu);
            itemSwapper = new ItemSwapper(hero, items, menu.ItemSwapMenu);
           // courierHelper = new CourierHelper(hero, items, menu.CourierHelperMenu);
        }

        #endregion
    }
}
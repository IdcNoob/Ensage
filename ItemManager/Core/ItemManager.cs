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

        private MenuManager menu;

        private RapierAbuse rapierAbuse;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.OnClose();
            items.OnClose();
            rapierAbuse.OnClose();
            itemSwapper.OnClose();
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            menu = new MenuManager();
            items = new Items(hero, menu.ItemSwapMenu);
            rapierAbuse = new RapierAbuse(hero, items, menu.RapierAbuseMenu);
            itemSwapper = new ItemSwapper(hero, items, menu.ItemSwapMenu);
        }

        #endregion
    }
}
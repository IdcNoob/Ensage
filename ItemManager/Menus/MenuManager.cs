namespace ItemManager.Menus
{
    using Ensage.Common.Menu;

    using ItemSwap;

    internal class MenuManager
    {
        #region Fields

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu("Item Manager", "itemManager", true);

            ItemSwapMenu = new ItemSwapMenu(menu);

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public ItemSwapMenu ItemSwapMenu { get; }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}
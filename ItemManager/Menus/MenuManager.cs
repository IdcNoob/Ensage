namespace ItemManager.Menus
{
    using CourierHelper;

    using Ensage.Common.Menu;

    using ItemSwap;

    internal class MenuManager
    {
        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu(" Item Manager", "itemManager", true, "courier_go_to_secretshop", true);

            ItemSwapMenu = new ItemSwapMenu(menu);
            //CourierHelperMenu = new CourierHelperMenu(menu);

            menu.AddToMainMenu();
        }

        #endregion

        #region Fields

        private readonly Menu menu;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion

        #region Public Properties

        public ItemSwapMenu ItemSwapMenu { get; }

        //public CourierHelperMenu CourierHelperMenu { get; }

        #endregion
    }
}
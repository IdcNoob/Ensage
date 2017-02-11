namespace Debugger.Menus
{
    using Dumping;

    using Ensage.Common.Menu;

    using OnAddRemove;

    using OnChange;

    using SharpDX;

    internal class MenuManager
    {
        #region Fields

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu =
                new Menu(" Debugger", "debugger", true, "chaos_knight_reality_rift", true).SetFontColor(
                    Color.PaleVioletRed);

            OnAddRemove = new OnAddRemoveMenu(menu);
            OnChangeMenu = new OnChangeMenu(menu);
            DumpingMenu = new DumpingMenu(menu);

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public DumpingMenu DumpingMenu { get; }

        public OnAddRemoveMenu OnAddRemove { get; }

        public OnChangeMenu OnChangeMenu { get; }

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}
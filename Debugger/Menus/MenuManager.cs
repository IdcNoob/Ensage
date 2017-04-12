namespace Debugger.Menus
{
    using Dumping;

    using Ensage.Common.Menu;

    using GameEvents;

    using OnAddRemove;

    using OnChange;

    using OnExecuteOrder;

    using SharpDX;

    internal class MenuManager
    {
        private readonly Menu menu;

        public MenuManager()
        {
            menu =
                new Menu(" Debugger", "debugger", true, "chaos_knight_reality_rift", true).SetFontColor(
                    Color.PaleVioletRed);

            OnAddRemove = new OnAddRemoveMenu(menu);
            OnChangeMenu = new OnChangeMenu(menu);
            OnExecuteOrderMenu = new OnExecuteOrderMenu(menu);
            GameEventsMenu = new GameEventsMenu(menu);
            DumpingMenu = new DumpingMenu(menu);

            menu.AddToMainMenu();
        }

        public DumpingMenu DumpingMenu { get; }

        public GameEventsMenu GameEventsMenu { get; }

        public OnAddRemoveMenu OnAddRemove { get; }

        public OnChangeMenu OnChangeMenu { get; }

        public OnExecuteOrderMenu OnExecuteOrderMenu { get; }

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }
    }
}
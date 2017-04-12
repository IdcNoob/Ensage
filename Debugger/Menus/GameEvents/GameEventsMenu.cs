namespace Debugger.Menus.GameEvents
{
    using Ensage.Common.Menu;

    internal class GameEventsMenu
    {
        public GameEventsMenu(Menu mainMenu)
        {
            var menu = new Menu("Game events", "onGameEvents");

            FireEvent = new FireEvent(menu);
            GcMessage = new GcMessage(menu);
            UiState = new UiState(menu);
            Message = new Message(menu);

            mainMenu.AddSubMenu(menu);
        }

        public FireEvent FireEvent { get; }

        public GcMessage GcMessage { get; }

        public Message Message { get; }

        public UiState UiState { get; }
    }
}
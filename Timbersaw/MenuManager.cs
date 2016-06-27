namespace Timbersaw
{
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem enabled;

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager(string heroName)
        {
            menu = new Menu("Timbersaw ?", "timbersaw", true, heroName, true);

            menu.AddItem(enabled = new MenuItem("enabled", "Enabled").SetValue(true));
            menu.AddItem(new MenuItem("comboKey", "Chase").SetValue(new KeyBind('F', KeyBindType.Press))).ValueChanged
                += (sender, arg) => { ComboEnabled = arg.GetNewValue<KeyBind>().Active; };

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool ComboEnabled { get; private set; }

        public bool IsEnabled => enabled.GetValue<bool>();

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}
namespace ExperienceTracker
{
    using Ensage.Common.Menu;

    internal class MenuManager
    {
        #region Fields

        private readonly MenuItem enabled;

        private readonly Menu menu;

        private readonly MenuItem simplified;

        private readonly MenuItem warningB;

        private readonly MenuItem warningG;

        private readonly MenuItem warningR;

        private readonly MenuItem warningSize;

        private readonly MenuItem warningTime;

        private readonly MenuItem warningX;

        private readonly MenuItem warningY;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu("Exp Tracker", "expTracker", true);

            menu.AddItem(enabled = new MenuItem("enabled", "Enabled")).SetValue(true);
            menu.AddItem(warningTime = new MenuItem("warningTime", "Show warning for (sec)"))
                .SetValue(new Slider(5, 1, 10));
            menu.AddItem(warningSize = new MenuItem("warningSize", "Warning size")).SetValue(new Slider(24, 10, 40));
            menu.AddItem(warningX = new MenuItem("warningX", "Warning x position")).SetValue(new Slider(-15, -100));
            menu.AddItem(warningY = new MenuItem("warningY", "Warning y position")).SetValue(new Slider(-25, -100));
            menu.AddItem(warningR = new MenuItem("warningR", "Warning red color")).SetValue(new Slider(255, 0, 255));
            menu.AddItem(warningG = new MenuItem("warningG", "Warning green color")).SetValue(new Slider(200, 0, 255));
            menu.AddItem(warningB = new MenuItem("warningB", "Warning blue color")).SetValue(new Slider(0, 0, 255));
            menu.AddItem(simplified = new MenuItem("simplified", "Simplified warning"))
                .SetValue(false)
                .SetTooltip("Will show only number of enemies");

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Properties

        public bool Enabled => enabled.IsActive();

        public bool SimplifiedWarning => simplified.IsActive();

        public int WarningBlueColor => warningB.GetValue<Slider>().Value;

        public int WarningGreenColor => warningG.GetValue<Slider>().Value;

        public int WarningRedColor => warningR.GetValue<Slider>().Value;

        public int WarningSize => warningSize.GetValue<Slider>().Value;

        public int WarningTime => warningTime.GetValue<Slider>().Value;

        public int WarningX => warningX.GetValue<Slider>().Value;

        public int WarningY => warningY.GetValue<Slider>().Value;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            menu.RemoveFromMainMenu();
        }

        #endregion
    }
}
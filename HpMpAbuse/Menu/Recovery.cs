namespace HpMpAbuse.Menu
{
    using Ensage.Common.Menu;

    internal class Recovery
    {
        #region Fields

        private readonly MenuItem forcePickEnemyDistance;

        private readonly MenuItem forcePickWhenMoved;

        private readonly MenuItem soulRingEnabled;

        private readonly MenuItem soulRingFountain;

        private bool active;

        #endregion

        #region Constructors and Destructors

        public Recovery(Menu mainMenu)
        {
            var menu = new Menu("Recovery Abuse", "recoveryAbuse", false, "item_bottle", true);

            menu.AddItem(new MenuItem("recoveryKey", "Recovery key").SetValue(new KeyBind('T', KeyBindType.Press)))
                .ValueChanged += (sender, args) => Active = args.GetNewValue<KeyBind>().Active;
            menu.AddItem(soulRingEnabled = new MenuItem("soulRingRecovery", "Use soul ring").SetValue(true))
                .SetTooltip("Will use thresholds from auto soul ring");
            menu.AddItem(
                soulRingFountain = new MenuItem("soulRingFountain", "Use soul ring at fountain").SetValue(true));

            var forcePick = new Menu("Force Item picking", "forcePick");
            forcePick.AddItem(forcePickWhenMoved = new MenuItem("forcePickMoved", "When hero moved").SetValue(true));
            forcePick.AddItem(
                forcePickEnemyDistance =
                new MenuItem("forcePickEnemyNearDistance", "When enemy in range").SetValue(new Slider(500, 0, 1000))
                    .SetTooltip("If enemy is closer then pick items"));

            menu.AddSubMenu(forcePick);

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
                if (active)
                {
                    Variables.Sleeper.Reset("Main");
                }
            }
        }

        public int ForcePickEnemyDistance => forcePickEnemyDistance.GetValue<Slider>().Value;

        public bool ForcePickWhenMovedEnabled => forcePickWhenMoved.IsActive();

        public bool SoulRingAtFountain => soulRingFountain.IsActive();

        public bool SoulRingEnabled => soulRingEnabled.IsActive();

        #endregion
    }
}
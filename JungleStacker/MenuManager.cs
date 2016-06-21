namespace JungleStacker
{
    using System;

    using Ensage.Common.Menu;

    using global::JungleStacker.Classes;

    internal class MenuArgs : EventArgs
    {
        #region Public Properties

        public bool Enabled { get; set; }

        #endregion
    }

    internal class MenuManager
    {
        #region Static Fields

        private static MenuItem debug;

        private static MenuItem enabled;

        #endregion

        #region Fields

        private readonly Menu menu = new Menu("Jungle Stacker", "jungleStacker", true);

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu.AddItem(
                enabled = new MenuItem("enableKey", "Enabled").SetValue(new KeyBind('P', KeyBindType.Toggle, true)))
                .ValueChanged += OnStateChange;
            menu.AddItem(new MenuItem("resetKey", "Reset stacks").SetValue(new KeyBind('0', KeyBindType.Press)))
                .SetTooltip("Set required stacks count to 1")
                .ValueChanged += StacksReset;
            menu.AddItem(new MenuItem("heroStack", "Stack with hero").SetValue(new KeyBind('K', KeyBindType.Press)))
                .SetTooltip("Will stack closest camp with your hero")
                .ValueChanged += OnHeroStackEabled;
            menu.AddItem(new MenuItem("forceAdd", "Force add unit").SetValue(new KeyBind('L', KeyBindType.Press)))
                .SetTooltip(
                    "Will add selected unit to controllables. Useful for ally dominated creep with shared control")
                .ValueChanged += OnMenuForceAdd;
            menu.AddItem(debug = new MenuItem("debug", "Debug").SetValue(false))
                .SetTooltip("Shows more information for debugging")
                .ValueChanged += OnDebugChange;

            menu.AddToMainMenu();
        }

        #endregion

        #region Public Events

        public event EventHandler OnForceAdd;

        public event EventHandler OnHeroStack;

        public event EventHandler<MenuArgs> OnProgramStateChange;

        public event EventHandler OnStacksReset;

        #endregion

        #region Public Properties

        public bool IsDebugEnabled => debug.IsActive();

        public bool IsEnabled => enabled.IsActive();

        #endregion

        #region Methods

        private static void OnDebugChange(object sender, OnValueChangeEventArgs arg)
        {
            Camp.Debug = Controllable.Debug = arg.GetNewValue<bool>();
        }

        private void OnHeroStackEabled(object sender, OnValueChangeEventArgs arg)
        {
            if (arg.GetNewValue<KeyBind>().Active)
            {
                var onHeroStack = OnHeroStack;
                onHeroStack?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnMenuForceAdd(object sender, OnValueChangeEventArgs arg)
        {
            if (arg.GetNewValue<KeyBind>().Active)
            {
                var onForceAdd = OnForceAdd;
                onForceAdd?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnStateChange(object sender, OnValueChangeEventArgs arg)
        {
            var onMenuChanged = OnProgramStateChange;
            onMenuChanged?.Invoke(this, new MenuArgs { Enabled = arg.GetNewValue<KeyBind>().Active });
        }

        private void StacksReset(object sender, OnValueChangeEventArgs arg)
        {
            if (arg.GetNewValue<KeyBind>().Active)
            {
                var onStacksReset = OnStacksReset;
                onStacksReset?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
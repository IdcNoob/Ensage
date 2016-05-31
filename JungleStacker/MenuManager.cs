namespace JungleStacker
{
    using System;

    using Ensage.Common.Menu;

    internal class MenuArgs : EventArgs
    {
        #region Public Properties

        public bool Enabled { get; set; }

        #endregion
    }

    internal class MenuManager
    {
        #region Fields

        private readonly Menu menu;

        #endregion

        #region Constructors and Destructors

        public MenuManager()
        {
            menu = new Menu("Jungle Stacker", "jungleStacker", true);
            menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true)).ValueChanged += OnStateChange;
            menu.AddItem(new MenuItem("heroStack", "Stack with hero").SetValue(new KeyBind('K', KeyBindType.Press)))
                .SetTooltip("Will stack closest camp with your hero")
                .ValueChanged += OnHeroStackEabled;
            menu.AddItem(new MenuItem("forceAdd", "Force add unit").SetValue(new KeyBind('L', KeyBindType.Press)))
                .SetTooltip(
                    "Will add selected unit to controllables, useful for ally dominated creep with shared control")
                .ValueChanged += OnMenuForceAdd;
            menu.AddToMainMenu();
        }

        #endregion

        #region Public Events

        public event EventHandler OnForceAdd;

        public event EventHandler OnHeroStack;

        public event EventHandler<MenuArgs> OnProgramStateChange;

        #endregion

        #region Public Properties

        public bool IsEnabled => menu.Item("enabled").GetValue<bool>();

        #endregion

        #region Methods

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
            onMenuChanged?.Invoke(this, new MenuArgs { Enabled = arg.GetNewValue<bool>() });
        }

        #endregion
    }
}
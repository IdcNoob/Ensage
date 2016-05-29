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
            this.menu = new Menu("Jungle Stacker", "jungleStacker", true);
            this.menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true)).ValueChanged += this.OnStateChange;
            this.menu.AddItem(new MenuItem("heroStack", "Stack with hero").SetValue(new KeyBind('K', KeyBindType.Press)))
                .SetTooltip("Will stack closest camp with your hero")
                .ValueChanged += this.OnHeroStackEabled;
            this.menu.AddItem(new MenuItem("forceAdd", "Force add unit").SetValue(new KeyBind('L', KeyBindType.Press)))
                .SetTooltip("Will add selected unit to controllables, useful for ally dominated creep with shared control")
                .ValueChanged += this.OnMenuForceAdd;
            this.menu.AddToMainMenu();
        }

        #endregion

        #region Public Events

        public event EventHandler OnHeroStack;
        public event EventHandler OnForceAdd;

        public event EventHandler<MenuArgs> OnProgramStateChange;

        #endregion

        #region Public Properties

        public bool IsEnabled
        {
            get
            {
                return this.menu.Item("enabled").GetValue<bool>();
            }
        }

        #endregion

        #region Methods

        private void OnHeroStackEabled(object sender, OnValueChangeEventArgs arg)
        {
            if (arg.GetNewValue<KeyBind>().Active)
            {
                var onHeroStack = this.OnHeroStack;
                if (onHeroStack != null)
                {
                    onHeroStack.Invoke(this, EventArgs.Empty);
                }
            }
        }
        private void OnMenuForceAdd(object sender, OnValueChangeEventArgs arg)
        {
            if (arg.GetNewValue<KeyBind>().Active)
            {
                var onForceAdd = this.OnForceAdd;
                if (onForceAdd != null)
                {
                    onForceAdd.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void OnStateChange(object sender, OnValueChangeEventArgs arg)
        {
            var onMenuChanged = this.OnProgramStateChange;
            if (onMenuChanged != null)
            {
                onMenuChanged.Invoke(this, new MenuArgs { Enabled = arg.GetNewValue<bool>() });
            }
        }

        #endregion
    }
}
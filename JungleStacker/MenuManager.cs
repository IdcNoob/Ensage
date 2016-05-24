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
            this.menu.AddItem(new MenuItem("enabled", "Enabled").SetValue(true)).ValueChanged += this.OnValueChanged;
            this.menu.AddToMainMenu();
        }

        #endregion

        #region Public Events

        public event EventHandler<MenuArgs> MenuChanged;

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

        protected virtual void OnMenuChanged(bool newValue)
        {
            var onMenuChanged = this.MenuChanged;
            if (onMenuChanged != null)
            {
                onMenuChanged.Invoke(this, new MenuArgs { Enabled = newValue });
            }
        }

        private void OnValueChanged(object sender, OnValueChangeEventArgs arg)
        {
            this.OnMenuChanged(arg.GetNewValue<bool>());
        }

        #endregion
    }
}
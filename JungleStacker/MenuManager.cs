namespace JungleStacker
{
    using System;

    using Classes;

    using Ensage.Common.Menu;

    internal class MenuArgs : EventArgs
    {
        public bool Enabled { get; set; }
    }

    internal class MenuManager
    {
        private static MenuItem debug;

        private static MenuItem enabled;

        private readonly Menu menu = new Menu("Jungle Stacker", "jungleStacker", true);

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
            menu.AddItem(
                    new MenuItem("forceAdd", "Force add/remove unit").SetValue(new KeyBind('L', KeyBindType.Press)))
                .SetTooltip(
                    "Will add/remove selected unit to/from controllables. Useful for ally dominated creep with shared control")
                .ValueChanged += OnMenuForceAdd;
            menu.AddItem(debug = new MenuItem("debug", "Debug").SetValue(false))
                .SetTooltip("Shows more information for debugging")
                .ValueChanged += OnDebugChange;

            menu.AddToMainMenu();
        }

        public event EventHandler OnForceAdd;

        public event EventHandler OnHeroStack;

        public event EventHandler<MenuArgs> OnProgramStateChange;

        public event EventHandler OnStacksReset;

        public bool IsDebugEnabled => debug.IsActive();

        public bool IsEnabled => enabled.IsActive();

        private static void OnDebugChange(object sender, OnValueChangeEventArgs arg)
        {
            Camp.Debug = Controllable.Debug = arg.GetNewValue<bool>();
        }

        private void OnHeroStackEabled(object sender, OnValueChangeEventArgs arg)
        {
            if (arg.GetNewValue<KeyBind>().Active)
            {
                OnHeroStack?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnMenuForceAdd(object sender, OnValueChangeEventArgs arg)
        {
            if (arg.GetNewValue<KeyBind>().Active)
            {
                OnForceAdd?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnStateChange(object sender, OnValueChangeEventArgs arg)
        {
            OnProgramStateChange?.Invoke(
                this,
                new MenuArgs
                {
                    Enabled = arg.GetNewValue<KeyBind>().Active
                });
        }

        private void StacksReset(object sender, OnValueChangeEventArgs arg)
        {
            if (arg.GetNewValue<KeyBind>().Active)
            {
                OnStacksReset?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
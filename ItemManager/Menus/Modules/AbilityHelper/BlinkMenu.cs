namespace ItemManager.Menus.Modules.AbilityHelper
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class BlinkMenu
    {
        public BlinkMenu(Menu mainMenu)
        {
            var menu = new Menu("Blink dagger", "blinkAdjustment");

            var enabled = new MenuItem("blinkAdjustment", "Maximize blink range").SetValue(true);
            menu.AddItem(enabled);
            enabled.ValueChanged += (sender, args) =>
                {
                    IsEnabled = args.GetNewValue<bool>();
                    OnEnabledChange?.Invoke(null, new BoolEventArgs(IsEnabled));
                };
            IsEnabled = enabled.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnEnabledChange;

        public bool IsEnabled { get; private set; }
    }
}
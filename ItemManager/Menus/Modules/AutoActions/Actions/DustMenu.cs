namespace ItemManager.Menus.Modules.AutoActions.Actions
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class DustMenu
    {
        public DustMenu(Menu mainMenu)
        {
            var menu = new Menu("Dust", "dustMenu");

            var enabled = new MenuItem("dustEnabled", "Enabled").SetValue(true);
            enabled.SetTooltip("Auto use dust of appearance");
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
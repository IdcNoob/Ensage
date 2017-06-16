namespace ItemManager.Menus.Modules.ItemSwap
{
    using System;

    using Ensage.Common.Menu;

    using EventArgs;

    internal class Auto
    {
        public Auto(Menu mainMenu)
        {
            var menu = new Menu("Auto swap", "autoSwapper");

            var autoScrollMove = new MenuItem("autoScrollMoveBackpack", "Town portal scroll").SetValue(false);
            autoScrollMove.SetTooltip("Auto swap tp scroll with most valuable backpack item when used");
            menu.AddItem(autoScrollMove);
            autoScrollMove.ValueChanged += (sender, args) =>
                {
                    SwapTpScroll = args.GetNewValue<bool>();
                    OnAutoMoveTpScrollChange?.Invoke(null, new BoolEventArgs(SwapTpScroll));
                };
            SwapTpScroll = autoScrollMove.IsActive();

            var autoAegis = new MenuItem("autoAegisMoveBackpack", "Aegis/Cheese on death/use").SetValue(false)
                .SetTooltip("Auto swap aegis and cheese with most valuable backpack item");
            menu.AddItem(autoAegis);
            autoAegis.ValueChanged += (sender, args) => SwapCheeseAegis = args.GetNewValue<bool>();
            SwapCheeseAegis = autoAegis.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> OnAutoMoveTpScrollChange;

        public bool SwapCheeseAegis { get; private set; }

        public bool SwapTpScroll { get; private set; }
    }
}
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
                    this.AutoMoveTpScrollChange?.Invoke(null, new BoolEventArgs(SwapTpScroll));
                };
            SwapTpScroll = autoScrollMove.IsActive();

            var autoAegis = new MenuItem("autoAegisMoveBackpack", "Aegis/Cheese on death/use").SetValue(false)
                .SetTooltip("Auto swap aegis and cheese with most valuable backpack item");
            menu.AddItem(autoAegis);
            autoAegis.ValueChanged += (sender, args) => SwapCheeseAegis = args.GetNewValue<bool>();
            SwapCheeseAegis = autoAegis.IsActive();

            var autoRaindrop = new MenuItem("autoRaindropMoveBackpack", "Raindrop on low charge").SetValue(false)
                .SetTooltip("Auto move raindrop to backpack when only 1 charge left");
            menu.AddItem(autoRaindrop);
            autoRaindrop.ValueChanged += (sender, args) =>
                {
                    this.SwapRaindrop = args.GetNewValue<bool>();
                    this.AutoMoveRaindropChange?.Invoke(null, new BoolEventArgs(SwapRaindrop));
                };
            SwapRaindrop = autoRaindrop.IsActive();

            var backpackItems = new MenuItem("autoBackpackItemsToInv", "Backpack items to inventory on death")
                .SetValue(false)
                .SetTooltip("Auto swap backpack items to inventory on death to reduce cooldown");
            menu.AddItem(backpackItems);
            backpackItems.ValueChanged += (sender, args) =>
                {
                    SwapBackpackItems = args.GetNewValue<bool>();
                    this.SwapBackpackItemsChange?.Invoke(null, new BoolEventArgs(SwapBackpackItems));
                };
            SwapBackpackItems = backpackItems.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public event EventHandler<BoolEventArgs> AutoMoveTpScrollChange;
        public event EventHandler<BoolEventArgs> AutoMoveRaindropChange;

        public event EventHandler<BoolEventArgs> SwapBackpackItemsChange;


        public bool SwapBackpackItems { get; private set; }

        public bool SwapCheeseAegis { get; private set; }
    
        public bool SwapRaindrop { get; private set; }

        public bool SwapTpScroll { get; private set; }
    }
}
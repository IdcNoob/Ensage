namespace ItemManager.Menus.ItemSwap
{
    using Ensage.Common.Menu;

    internal class Auto
    {
        #region Constructors and Destructors

        public Auto(Menu mainMenu)
        {
            var menu = new Menu("Auto swap", "autoSwapper");

            var autoScrollMove =
                new MenuItem("autoScrollMoveBackpack", "Town portal scroll").SetValue(false)
                    .SetTooltip("Auto swap tp scroll with most valuable backpack item when used");
            menu.AddItem(autoScrollMove);
            autoScrollMove.ValueChanged += (sender, args) => SwapTpScroll = args.GetNewValue<bool>();
            SwapTpScroll = autoScrollMove.IsActive();

            var tpScrollAbuse =
                new MenuItem("tpAbuse", "Town portal scroll abuse").SetValue(false)
                    .SetTooltip("Removes town portal scroll cooldown");
            menu.AddItem(tpScrollAbuse);
            tpScrollAbuse.ValueChanged += (sender, args) => TpScrollAbuse = args.GetNewValue<bool>();
            TpScrollAbuse = tpScrollAbuse.IsActive();

            var autoAegis =
                new MenuItem("autoAegisMoveBackpack", "Aegis/Cheese").SetValue(false)
                    .SetTooltip("Auto swap aegis and cheese with most valuable backpack item when used");
            menu.AddItem(autoAegis);
            autoAegis.ValueChanged += (sender, args) => SwapCheeseAegis = args.GetNewValue<bool>();
            SwapCheeseAegis = autoAegis.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        public bool TpScrollAbuse { get; private set; }

        #region Public Properties

        public bool SwapCheeseAegis { get; private set; }

        public bool SwapTpScroll { get; private set; }

        #endregion
    }
}
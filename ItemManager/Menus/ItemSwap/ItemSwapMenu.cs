namespace ItemManager.Menus.ItemSwap
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class ItemSwapMenu
    {
        #region Fields

        private readonly List<string> addedItems = new List<string>();

        #endregion

        #region Constructors and Destructors

        public ItemSwapMenu(Menu mainMenu)
        {
            var menu = new Menu("Item swapper", "itemSwapper");

            Backpack = new Backpack(menu);
            Stash = new Stash(menu);

            var force =
                new MenuItem("forceSwapBackpack", "Force swap").SetValue(false)
                    .SetTooltip("Swap even if no free slots available");
            menu.AddItem(force);
            force.ValueChanged += (sender, args) => ForceItemSwap = args.GetNewValue<bool>();
            ForceItemSwap = force.IsActive();

            var autoScrollMove =
                new MenuItem("autoScrollMoveBackpack", "Auto swap tp scroll").SetValue(false)
                    .SetTooltip("Auto swap tp scroll with most valuable backpack item when used");
            menu.AddItem(autoScrollMove);
            autoScrollMove.ValueChanged += (sender, args) => AutoSwapTpScroll = args.GetNewValue<bool>();
            AutoSwapTpScroll = autoScrollMove.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Properties

        public bool AutoSwapTpScroll { get; private set; }

        public Backpack Backpack { get; }

        public bool ForceItemSwap { get; private set; }

        public Stash Stash { get; }

        #endregion

        #region Public Methods and Operators

        public void AddItem(string itemName)
        {
            if (addedItems.Contains(itemName))
            {
                return;
            }

            addedItems.Add(itemName);
            Backpack.AddItem(itemName);
            Stash.AddItem(itemName);
        }

        public void RemoveItem(string itemName)
        {
            if (!addedItems.Contains(itemName))
            {
                return;
            }

            addedItems.Remove(itemName);
            Backpack.RemoveItem(itemName);
            Stash.RemoveItem(itemName);
        }

        #endregion
    }
}
namespace ItemManager.Menus.ItemSwap
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class ItemSwapMenu
    {
        #region Constructors and Destructors

        public ItemSwapMenu(Menu mainMenu)
        {
            var menu = new Menu(" Item swapper", "itemSwapper", false, "vengefulspirit_nether_swap", true);

            Backpack = new Backpack(menu);
            Stash = new Stash(menu);
            Auto = new Auto(menu);

            var force =
                new MenuItem("forceSwapBackpack", "Force swap").SetValue(false)
                    .SetTooltip("Swap even if no free slots available");
            menu.AddItem(force);
            force.ValueChanged += (sender, args) => ForceItemSwap = args.GetNewValue<bool>();
            ForceItemSwap = force.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Fields

        private readonly List<string> addedItems = new List<string>();

        #endregion

        #region Public Properties

        public Auto Auto { get; }

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
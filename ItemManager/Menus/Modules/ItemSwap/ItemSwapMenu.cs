namespace ItemManager.Menus.Modules.ItemSwap
{
    using System.Collections.Generic;

    using Ensage.Common.Menu;

    internal class ItemSwapMenu
    {
        private readonly List<string> addedItems = new List<string>();

        public ItemSwapMenu(Menu mainMenu)
        {
            var menu = new Menu("Item swapper", "itemSwapper");

            Backpack = new Backpack(menu);
            Stash = new Stash(menu);
            Courier = new Courier(menu);
            Auto = new Auto(menu);

            var force = new MenuItem("forceSwapBackpack", "Force swap").SetValue(false)
                .SetTooltip("Swap even if no free slots available (only for backpack and stash swaps)");
            menu.AddItem(force);
            force.ValueChanged += (sender, args) => ForceItemSwap = args.GetNewValue<bool>();
            ForceItemSwap = force.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        public Auto Auto { get; }

        public Backpack Backpack { get; }

        public Courier Courier { get; }

        public bool ForceItemSwap { get; private set; }

        public Stash Stash { get; }

        public void AddItem(string itemName)
        {
            if (addedItems.Contains(itemName))
            {
                return;
            }

            addedItems.Add(itemName);
            Backpack.AddItem(itemName);
            Stash.AddItem(itemName);
            Courier.AddItem(itemName);
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
            Courier.RemoveItem(itemName);
        }
    }
}
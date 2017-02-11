namespace Debugger.Menus.Dumping
{
    using System;

    using Ensage.Common.Menu;

    using SharpDX;

    internal class Items
    {
        #region Constructors and Destructors

        public Items(Menu mainMenu)
        {
            var menu = new Menu("Items", "itemsDumpMenu");

            var dump = new MenuItem("items", "Get unit items").SetValue(false);
            menu.AddItem(dump);
            dump.ValueChanged += (sender, args) => { OnDump?.Invoke(this, EventArgs.Empty); };

            menu.AddItem(new MenuItem("itemsInfo", "Settings")).SetFontColor(Color.Yellow);

            var inventoryItems = new MenuItem("inventoryItems", "Show inventory items").SetValue(true);
            menu.AddItem(inventoryItems);
            inventoryItems.ValueChanged += (sender, args) => ShowInventoryItems = args.GetNewValue<bool>();
            ShowInventoryItems = inventoryItems.IsActive();

            var backpackItems = new MenuItem("backpackItems", "Show backpack items").SetValue(false);
            menu.AddItem(backpackItems);
            backpackItems.ValueChanged += (sender, args) => ShowBackpackItems = args.GetNewValue<bool>();
            ShowBackpackItems = backpackItems.IsActive();

            var stashItems = new MenuItem("stashItems", "Show stash items").SetValue(false);
            menu.AddItem(stashItems);
            stashItems.ValueChanged += (sender, args) => ShowStashItems = args.GetNewValue<bool>();
            ShowStashItems = stashItems.IsActive();

            var showManaCost = new MenuItem("manaItems", "Show mana cost").SetValue(false);
            menu.AddItem(showManaCost);
            showManaCost.ValueChanged += (sender, args) => ShowManaCost = args.GetNewValue<bool>();
            ShowManaCost = showManaCost.IsActive();

            var showCastRange = new MenuItem("rangeItems", "Show cast range").SetValue(false);
            menu.AddItem(showCastRange);
            showCastRange.ValueChanged += (sender, args) => ShowCastRange = args.GetNewValue<bool>();
            ShowCastRange = showCastRange.IsActive();

            var showBehavior = new MenuItem("behaviorItems", "Show behavior").SetValue(false);
            menu.AddItem(showBehavior);
            showBehavior.ValueChanged += (sender, args) => ShowBehavior = args.GetNewValue<bool>();
            ShowBehavior = showBehavior.IsActive();

            var showTargetType = new MenuItem("targetTypeItems", "Show target type").SetValue(false);
            menu.AddItem(showTargetType);
            showTargetType.ValueChanged += (sender, args) => ShowTargetType = args.GetNewValue<bool>();
            ShowTargetType = showTargetType.IsActive();

            var showSpecialData = new MenuItem("specialItems", "Show all special data").SetValue(false);
            menu.AddItem(showSpecialData);
            showSpecialData.ValueChanged += (sender, args) => ShowSpecialData = args.GetNewValue<bool>();
            ShowSpecialData = showSpecialData.IsActive();

            mainMenu.AddSubMenu(menu);
        }

        #endregion

        #region Public Events

        public event EventHandler OnDump;

        #endregion

        #region Public Properties

        public bool ShowBackpackItems { get; private set; }

        public bool ShowBehavior { get; private set; }

        public bool ShowCastRange { get; private set; }

        public bool ShowInventoryItems { get; private set; }

        public bool ShowManaCost { get; private set; }

        public bool ShowSpecialData { get; private set; }

        public bool ShowStashItems { get; private set; }

        public bool ShowTargetType { get; private set; }

        #endregion
    }
}
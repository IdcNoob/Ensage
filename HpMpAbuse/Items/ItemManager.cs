namespace HpMpAbuse.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using HpMpAbuse.Helpers;
    using HpMpAbuse.Menu;

    internal class ItemManager
    {
        #region Fields

        public readonly List<UsableItem> UsableItems = new List<UsableItem>();

        private readonly List<uint> droppedItems = new List<uint>();

        private readonly Dictionary<ItemSlot, Item> itemSlots = new Dictionary<ItemSlot, Item>();

        private readonly ItemsStats itemsStats = new ItemsStats();

        #endregion

        #region Constructors and Destructors

        public ItemManager()
        {
            PowerTreads = new PowerTreads("item_power_treads");
            TranquilBoots = new TranquilBoots("item_tranquil_boots");

            UsableItems.Add(new Mekansm("item_mekansm"));
            UsableItems.Add(new ArcaneBoots("item_arcane_boots"));
            UsableItems.Add(new GuardianGreaves("item_guardian_greaves"));
            UsableItems.Add(SoulRing = new SoulRing("item_soul_ring"));
            UsableItems.Add(Bottle = new Bottle("item_bottle"));
            UsableItems.Add(StashBottle = new StashBottle("item_bottle"));
            UsableItems.Add(new MagicStick("item_magic_stick"));
            UsableItems.Add(new UrnOfShadows("item_urn_of_shadows"));

            Game.OnIngameUpdate += OnUpdate;
        }

        #endregion

        #region Public Properties

        public Bottle Bottle { get; set; }

        public PowerTreads PowerTreads { get; }

        public SoulRing SoulRing { get; }

        public StashBottle StashBottle { get; }

        public ItemSlot StashBottleSlot { get; private set; }

        public bool StashBottleTaken { get; private set; }

        public TranquilBoots TranquilBoots { get; }

        #endregion

        #region Properties

        private static Hero Hero => Variables.Hero;

        private static MenuManager Menu => Variables.Menu;

        private static MultiSleeper Sleeper => Variables.Sleeper;

        #endregion

        #region Public Methods and Operators

        public void DropItem(Item item, bool queue = true)
        {
            SaveItemSlot(item);
            droppedItems.Add(item.Handle);
            Hero.DropItem(item, Hero.Position, queue);
        }

        public void DropItems(ItemsStats.Stats dropStats, Item ignoredItem = null)
        {
            if (dropStats == ItemsStats.Stats.None)
            {
                return;
            }

            Hero.Inventory.Items.Where(x => !x.Equals(ignoredItem) && itemsStats.GetStats(x).HasFlag(dropStats))
                .ForEach(x => DropItem(x));
        }

        public int DroppedItemsCount()
        {
            return droppedItems.Count;
        }

        public void OnClose()
        {
            Game.OnIngameUpdate -= OnUpdate;
            UsableItems.Clear();
        }

        public void PickUpItems(params string[] itemNames)
        {
            var items = ObjectManager.GetEntities<PhysicalItem>().Where(x => itemNames.Contains(x.Item.StoredName()));
            foreach (var item in items)
            {
                Hero.PickUpItem(item);
            }
        }

        public void PickUpItems(bool all = false)
        {
            if (StashBottleTaken)
            {
                Bottle.Item?.MoveItem(StashBottleSlot);
                Bottle.Item = null;
                StashBottleTaken = false;
            }

            if (!droppedItems.Any() && !all)
            {
                return;
            }

            var items =
                ObjectManager.GetEntities<PhysicalItem>()
                    .Where(x => x.Distance2D(Hero) < 350 && (all || droppedItems.Contains(x.Item.Handle)))
                    .ToList();

            var count = items.Count;

            if (count <= 0)
            {
                return;
            }

            for (var i = 0; i < count; i++)
            {
                Hero.PickUpItem(items[i], i != 0);
            }

            foreach (var itemSlot in itemSlots)
            {
                itemSlot.Value.MoveItem(itemSlot.Key);
            }

            itemSlots.Clear();
            droppedItems.Clear();
        }

        public void PickUpItemsOnMove(ExecuteOrderEventArgs args)
        {
            if (Sleeper.Sleeping("PickOnMove"))
            {
                args.Process = false;
            }

            if (!droppedItems.Any())
            {
                return;
            }

            args.Process = false;
            PickUpItems();
            Hero.Move(args.TargetPosition, true);
            Sleeper.Sleep(2000, "Main");
            Sleeper.Sleep(300 + Game.Ping, "PickOnMove");
        }

        public void TakeBottleFromStash()
        {
            if (StashBottle.Item == null)
            {
                return;
            }

            SaveBottleStashSlot();

            for (var i = 0; i < 6; i++)
            {
                var currentSlot = (ItemSlot)i;
                var currentItem = Hero.Inventory.GetItem(currentSlot);

                if (currentItem == null || !UsableItems.Select(x => x.Item).Contains(currentItem))
                {
                    StashBottle.Item.MoveItem(currentSlot);
                    break;
                }
            }

            StashBottleTaken = true;
            Bottle.Item = StashBottle.Item;
            Bottle.SetSleep(300 + Game.Ping);
            Sleeper.Sleep(300 + Game.Ping, this);
        }

        #endregion

        #region Methods

        private void OnUpdate(EventArgs args)
        {
            if (!Sleeper.Sleeping(TranquilBoots))
            {
                if (Menu.TranquilBoots.CombineActive)
                {
                    TranquilBoots.FindItem();
                }
                Sleeper.Sleep(500, TranquilBoots);
            }

            if (Sleeper.Sleeping(this))
            {
                return;
            }

            Sleeper.Sleep(3000, this);

            foreach (var item in UsableItems)
            {
                item.FindItem();
            }

            PowerTreads.FindItem();
            TranquilBoots.FindItem();
        }

        private void SaveBottleStashSlot()
        {
            for (var i = 6; i < 12; i++)
            {
                var currentSlot = (ItemSlot)i;
                var currentItem = Hero.Inventory.GetItem(currentSlot);

                if (currentItem == null)
                {
                    continue;
                }

                if (Hero.Inventory.GetItem(currentSlot).Equals(StashBottle.Item))
                {
                    StashBottleSlot = currentSlot;
                    break;
                }
            }
        }

        private void SaveItemSlot(Item item)
        {
            for (var i = 0; i < 6; i++)
            {
                var currentSlot = (ItemSlot)i;
                if (itemSlots.ContainsKey(currentSlot))
                {
                    continue;
                }
                var currentItem = Hero.Inventory.GetItem(currentSlot);
                if (currentItem == null || !currentItem.Equals(item))
                {
                    continue;
                }
                itemSlots.Add(currentSlot, item);
                break;
            }
        }

        #endregion
    }
}
namespace ItemManager.Core.Modules.ItemSwapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Menus;
    using Menus.Modules.ItemSwap;

    using Utils;

    using Courier = Ensage.Courier;

    [Module]
    internal class ItemSwapper : IDisposable
    {
        private readonly Manager manager;

        private readonly ItemSwapMenu menu;

        private readonly Sleeper sleeper;

        private readonly IUpdateHandler updateHandler;

        private bool courierFollowing;

        private bool itemsMovedToCourier;

        private List<Item> swappedItems = new List<Item>();

        public ItemSwapper(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.ItemSwapMenu;
            sleeper = new Sleeper();

            updateHandler = UpdateManager.Subscribe(OnUpdate, 100, false);
            this.menu.Backpack.OnSwap += BackpackOnSwap;
            this.menu.Stash.OnSwap += StashOnSwap;
            this.menu.Courier.OnSwap += CourierOnSwap;
            Unit.OnModifierAdded += OnModifierAdded;
            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            manager.OnItemAdd += OnItemAdd;
            manager.OnItemRemove += OnItemRemove;
        }

        public void Dispose()
        {
            menu.Backpack.OnSwap -= BackpackOnSwap;
            menu.Stash.OnSwap -= StashOnSwap;
            menu.Courier.OnSwap -= CourierOnSwap;
            Unit.OnModifierAdded -= OnModifierAdded;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            manager.OnItemAdd -= OnItemAdd;
            manager.OnItemRemove -= OnItemRemove;
        }

        private void BackpackOnSwap(object sender, EventArgs eventArgs)
        {
            var inventoryItems = manager.MyHero.GetItems(ItemStoredPlace.Inventory)
                .Where(x => menu.Backpack.ItemEnabled(x.Name) && x.CanBeMovedToBackpack())
                .ToList();

            var backpackItems = manager.MyHero.GetItems(ItemStoredPlace.Backpack)
                .Where(x => menu.Backpack.ItemEnabled(x.Name))
                .ToList();

            MoveItems(inventoryItems, backpackItems, ItemStoredPlace.Backpack);
            MoveItems(backpackItems, inventoryItems, ItemStoredPlace.Inventory);
        }

        private void CourierOnSwap(object sender, EventArgs eventArgs)
        {
            if (updateHandler.IsEnabled)
            {
                courierFollowing = false;
                itemsMovedToCourier = false;
                return;
            }

            updateHandler.IsEnabled = true;
        }

        private void DisableCourierItemSwap()
        {
            courierFollowing = false;
            itemsMovedToCourier = false;
            updateHandler.IsEnabled = false;
        }

        private void MoveItem(
            Item item,
            ItemSlot slot,
            ICollection<ItemSlot> freeSlots,
            ICollection<ItemSlot> allSlots,
            ItemStoredPlace direction)
        {
            if (direction != ItemStoredPlace.Inventory)
            {
                manager.MyHero.SaveItemSlot(item);
            }
            else
            {
                var savedSlot = manager.MyHero.GetSavedSlot(item);
                if (savedSlot != null)
                {
                    slot = savedSlot.Value;
                }
            }

            item.MoveItem(slot);
            freeSlots.Remove(slot);
            allSlots.Remove(slot);
        }

        private void MoveItems(Unit source, Unit target, IReadOnlyCollection<Item> itemsToMove)
        {
            if (!itemsToMove.Any())
            {
                return;
            }

            var count = Math.Min(target.Inventory.FreeInventorySlots.Count(), itemsToMove.Count);
            for (var i = 0; i < count; i++)
            {
                source.GiveItem(itemsToMove.ElementAt(i), target, i != 0);
            }
        }

        private void MoveItems(IEnumerable<Item> from, ICollection<Item> to, ItemStoredPlace direction)
        {
            var freeSlots = new List<ItemSlot>();
            var allSlots = new List<ItemSlot>();

            switch (direction)
            {
                case ItemStoredPlace.Inventory:
                {
                    // ReSharper disable LoopCanBeConvertedToQuery
                    foreach (var itemSlot in Enum.GetValues(typeof(InventorySlot)).Cast<ItemSlot>())
                    {
                        allSlots.Add(itemSlot);
                    }
                    freeSlots.AddRange(manager.MyHero.Inventory.FreeInventorySlots);
                    break;
                }
                case ItemStoredPlace.Backpack:
                {
                    foreach (var itemSlot in Enum.GetValues(typeof(BackpackSlot)).Cast<ItemSlot>())
                    {
                        allSlots.Add(itemSlot);
                    }
                    freeSlots.AddRange(manager.MyHero.Inventory.FreeBackpackSlots);
                    break;
                }
                case ItemStoredPlace.Stash:
                {
                    foreach (var itemSlot in Enum.GetValues(typeof(StashSlot)).Cast<ItemSlot>())
                    {
                        allSlots.Add(itemSlot);
                    }
                    // ReSharper restore LoopCanBeConvertedToQuery
                    freeSlots.AddRange(manager.MyHero.Inventory.FreeStashSlots);
                    break;
                }
            }

            foreach (var item in from)
            {
                var swapItem = to.FirstOrDefault();

                if (swapItem != null)
                {
                    var slot = manager.MyHero.GetSlot(swapItem.Handle, direction);
                    if (slot != null)
                    {
                        item.MoveItem(slot.Value);
                    }

                    to.Remove(swapItem);
                }
                else
                {
                    if (freeSlots.Any())
                    {
                        MoveItem(item, freeSlots.First(), freeSlots, allSlots, direction);
                    }
                    else if (allSlots.Any() && menu.ForceItemSwap)
                    {
                        MoveItem(item, allSlots.First(), freeSlots, allSlots, direction);
                    }
                }
            }
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (!updateHandler.IsEnabled || args.OldValue == args.NewValue || args.PropertyName != "m_nCourierState")
            {
                return;
            }

            var courier = sender as Courier;
            if (courier == null || !courier.IsAlive)
            {
                return;
            }

            DisableCourierItemSwap();
        }

        private void OnItemAdd(object sender, ItemEventArgs itemEventArgs)
        {
            if (!itemEventArgs.IsMine || itemEventArgs.Item.IsRecipe)
            {
                return;
            }

            menu.AddItem(itemEventArgs.Item.Name);
        }

        private void OnItemRemove(object sender, ItemEventArgs itemEventArgs)
        {
            var item = itemEventArgs.Item;

            if (!itemEventArgs.IsMine || item.IsRecipe || manager.MyHero.GetItems(ItemStoredPlace.Any)
                    .Any(x => x.Id == item.Id))
            {
                return;
            }

            menu.RemoveItem(itemEventArgs.Item.Name);

            if (!menu.Auto.SwapCheeseAegis || item.Id != AbilityId.item_cheese && item.Id != AbilityId.item_aegis)
            {
                return;
            }

            var freeSlots = manager.MyHero.Inventory.FreeInventorySlots.ToList();
            if (!freeSlots.Any())
            {
                return;
            }

            var backpackItem = manager.MyHero.GetBestBackpackItem();
            backpackItem?.MoveItem(freeSlots.First());
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (!menu.Auto.SwapTpScroll || sender.Handle != manager.MyHero.Handle
                || args.Modifier.TextureName != "item_tpscroll")
            {
                return;
            }

            var backpackItem = manager.MyHero.GetBestBackpackItem();
            if (backpackItem == null)
            {
                return;
            }

            var scrollSlot = manager.MyHero.GetSlot(AbilityId.item_tpscroll, ItemStoredPlace.Inventory);
            if (scrollSlot != null)
            {
                backpackItem.MoveItem(scrollSlot.Value);
            }
        }

        private void OnUpdate()
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            var courier =
                EntityManager<Courier>.Entities.FirstOrDefault(
                    x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team);
            if (courier == null || !manager.MyHero.IsAlive)
            {
                DisableCourierItemSwap();
                return;
            }

            var inventoryItems = manager.MyHero.GetItems(ItemStoredPlace.Inventory)
                .Where(x => menu.Courier.ItemEnabled(x.Name) && x.CanBeMovedToBackpack())
                .ToList();

            var courierItems = courier.Inventory.Items
                .Where(x => x.Purchaser?.Hero?.Handle == manager.MyHero.Handle && menu.Courier.ItemEnabled(x.Name))
                .ToList();

            if (!inventoryItems.Any() && !courierItems.Any())
            {
                DisableCourierItemSwap();
                return;
            }

            if (!courierFollowing)
            {
                courier.Follow(manager.MyHero.Hero);
                courierFollowing = true;
            }

            if (courier.Distance2D(manager.MyHero.Position) > 250)
            {
                sleeper.Sleep(200);
                return;
            }

            if (!itemsMovedToCourier)
            {
                swappedItems = inventoryItems;
                itemsMovedToCourier = true;

                if (inventoryItems.Any())
                {
                    MoveItems(manager.MyHero.Hero, courier, inventoryItems);
                    sleeper.Sleep(300);
                    return;
                }
            }

            MoveItems(courier, manager.MyHero.Hero, courierItems.Where(x => !swappedItems.Contains(x)).ToList());
            DisableCourierItemSwap();
        }

        private void StashOnSwap(object sender, EventArgs eventArgs)
        {
            if (!manager.MyHero.IsAtBase())
            {
                return;
            }

            var inventoryItems = manager.MyHero.GetItems(ItemStoredPlace.Inventory)
                .Where(x => menu.Stash.ItemEnabled(x.Name) && x.CanBeMovedToBackpack())
                .ToList();

            var stashItems = manager.MyHero.GetItems(ItemStoredPlace.Stash)
                .Where(x => menu.Stash.ItemEnabled(x.Name))
                .ToList();

            MoveItems(inventoryItems, stashItems, ItemStoredPlace.Stash);
            MoveItems(stashItems, inventoryItems, ItemStoredPlace.Inventory);
        }
    }
}
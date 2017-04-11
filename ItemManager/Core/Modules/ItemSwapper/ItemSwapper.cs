namespace ItemManager.Core.Modules.ItemSwapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Extensions;

    using Menus.Modules.ItemSwap;

    using Utils;

    using Courier = Ensage.Courier;

    internal class ItemSwapper
    {
        private readonly Hero hero;

        private readonly ItemManager items;

        private readonly ItemSwapMenu menu;

        private bool couierSwapActive;

        public ItemSwapper(Hero myHero, ItemManager itemManager, ItemSwapMenu itemSwapMenu)
        {
            hero = myHero;
            menu = itemSwapMenu;
            items = itemManager;

            menu.Backpack.OnSwap += BackpackOnSwap;
            menu.Stash.OnSwap += StashOnSwap;
            menu.Courier.OnSwap += CourierOnSwap;

            Unit.OnModifierAdded += OnModifierAdded;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
        }

        public void OnClose()
        {
            menu.Backpack.OnSwap -= BackpackOnSwap;
            menu.Stash.OnSwap -= StashOnSwap;
            menu.Courier.OnSwap -= CourierOnSwap;

            Unit.OnModifierAdded -= OnModifierAdded;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
        }

        private void BackpackOnSwap(object sender, EventArgs eventArgs)
        {
            var inventoryItems = items.GetMyItems(ItemUtils.StoredPlace.Inventory)
                .Where(x => menu.Backpack.ItemEnabled(x.Name) && x.CanBeMoved())
                .ToList();

            var backpackItems = items.GetMyItems(ItemUtils.StoredPlace.Backpack)
                .Where(x => menu.Backpack.ItemEnabled(x.Name))
                .ToList();

            MoveItems(inventoryItems, backpackItems, ItemUtils.StoredPlace.Backpack);
            MoveItems(backpackItems, inventoryItems, ItemUtils.StoredPlace.Inventory);
        }

        private async void CourierOnSwap(object sender, EventArgs eventArgs)
        {
            if (couierSwapActive)
            {
                return;
            }

            var courier = ObjectManager.GetEntities<Courier>()
                .FirstOrDefault(x => x.IsValid && x.Team == hero.Team && x.IsAlive);
            if (courier == null)
            {
                return;
            }

            var inventoryItems = items.GetMyItems(ItemUtils.StoredPlace.Inventory)
                .Where(x => menu.Courier.ItemEnabled(x.Name) && x.CanBeMoved())
                .ToList();

            var courierItems = courier.Inventory.Items.Where(x => menu.Courier.ItemEnabled(x.Name)).ToList();

            couierSwapActive = true;

            courier.Follow(hero);
            await Task.Delay(200);

            while (courier.Distance2D(hero) > 250)
            {
                if (!couierSwapActive)
                {
                    return;
                }

                await Task.Delay(200);
            }

            MoveItem(hero, courier, inventoryItems);
            await Task.Delay(300);
            MoveItem(courier, hero, courierItems);

            couierSwapActive = false;
        }

        private void MoveItem(Unit source, Unit target, ICollection<Item> itemsToMove)
        {
            var count = Math.Min(target.Inventory.FreeInventorySlots.Count(), itemsToMove.Count);
            for (var i = 0; i < count; i++)
            {
                source.GiveItem(itemsToMove.ElementAt(i), target, i != 0);
            }
        }

        private void MoveItem(
            Item item,
            ItemSlot slot,
            ICollection<ItemSlot> freeSlots,
            ICollection<ItemSlot> allSlots,
            ItemUtils.StoredPlace direction)
        {
            if (direction != ItemUtils.StoredPlace.Inventory)
            {
                items.SaveItemSlot(item);
            }
            else
            {
                var savedSlot = items.GetSavedSlot(item);
                if (savedSlot != null)
                {
                    slot = savedSlot.Value;
                }
            }

            item.MoveItem(slot);
            freeSlots.Remove(slot);
            allSlots.Remove(slot);
        }

        private void MoveItems(IEnumerable<Item> from, ICollection<Item> to, ItemUtils.StoredPlace direction)
        {
            var freeSlots = new List<ItemSlot>();
            var allSlots = new List<ItemSlot>();

            switch (direction)
            {
                case ItemUtils.StoredPlace.Inventory:
                {
                    freeSlots.AddRange(hero.Inventory.FreeInventorySlots);
                    allSlots.AddRange(InventoryUtils.InventorySlots);
                    break;
                }
                case ItemUtils.StoredPlace.Backpack:
                {
                    freeSlots.AddRange(hero.Inventory.FreeBackpackSlots);
                    allSlots.AddRange(InventoryUtils.BackpackSlots);
                    break;
                }
                case ItemUtils.StoredPlace.Stash:
                {
                    freeSlots.AddRange(hero.Inventory.FreeStashSlots);
                    allSlots.AddRange(InventoryUtils.StashSlots);
                    break;
                }
            }

            foreach (var item in from)
            {
                var swapItem = to.FirstOrDefault();

                if (swapItem != null)
                {
                    var slot = items.GetSlot(swapItem.Id, direction);
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
            if (!couierSwapActive || args.OldValue == args.NewValue || args.PropertyName != "m_nCourierState")
            {
                return;
            }

            var courier = sender as Courier;
            if (courier == null || !courier.IsAlive)
            {
                return;
            }

            couierSwapActive = false;
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (!menu.Auto.SwapTpScroll || sender.Handle != hero.Handle || args.Modifier.TextureName != "item_tpscroll")
            {
                return;
            }

            var backpackItem = items.GetBestBackpackItem();

            if (backpackItem == null)
            {
                return;
            }

            var scrollSlot = items.GetSlot(AbilityId.item_tpscroll, ItemUtils.StoredPlace.Inventory);
            if (scrollSlot != null)
            {
                backpackItem.MoveItem(scrollSlot.Value);
            }
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            if (!menu.Auto.SwapCheeseAegis || args.Entity.Owner?.Handle != hero.Handle)
            {
                return;
            }

            var item = args.Entity as Item;
            if (item == null || item.Id != AbilityId.item_cheese && item.Id != AbilityId.item_aegis)
            {
                return;
            }

            var freeSlots = hero.Inventory.FreeInventorySlots.ToList();
            if (!freeSlots.Any())
            {
                return;
            }

            var backpackItem = items.GetBestBackpackItem();
            backpackItem?.MoveItem(freeSlots.First());
        }

        private void StashOnSwap(object sender, EventArgs eventArgs)
        {
            if (hero.ActiveShop != ShopType.Base)
            {
                return;
            }

            var inventoryItems = items.GetMyItems(ItemUtils.StoredPlace.Inventory)
                .Where(x => menu.Stash.ItemEnabled(x.Name) && x.CanBeMoved())
                .ToList();

            var stashItems = items.GetMyItems(ItemUtils.StoredPlace.Stash)
                .Where(x => menu.Stash.ItemEnabled(x.Name))
                .ToList();

            MoveItems(inventoryItems, stashItems, ItemUtils.StoredPlace.Stash);
            MoveItems(stashItems, inventoryItems, ItemUtils.StoredPlace.Inventory);
        }
    }
}
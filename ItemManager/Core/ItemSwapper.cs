namespace ItemManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;

    using Menus.ItemSwap;

    internal class ItemSwapper
    {
        #region Public Methods and Operators

        public void OnClose()
        {
            menu.Backpack.OnSwap -= BackpackOnSwap;
            menu.Stash.OnSwap -= StashOnSwap;
            Unit.OnModifierAdded -= OnModifierAdded;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        #endregion

        #region Fields

        private readonly Hero hero;

        private readonly Items items;

        private readonly ItemSwapMenu menu;

        #endregion

        #region Constructors and Destructors

        public ItemSwapper(Hero myHero, Items myItems, ItemSwapMenu itemSwapMenu)
        {
            hero = myHero;
            menu = itemSwapMenu;
            items = myItems;

            itemSwapMenu.Backpack.OnSwap += BackpackOnSwap;
            itemSwapMenu.Stash.OnSwap += StashOnSwap;
            Unit.OnModifierAdded += OnModifierAdded;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.Auto.TpScrollAbuse || args.Order != Order.AbilityLocation
                || args.Ability.AbilityId != AbilityId.item_tpscroll)
            {
                return;
            }

            var tpScroll = (Item)args.Ability;
            var slot = items.GetSlot(tpScroll.AbilityId, Items.StoredPlace.Inventory);
            tpScroll.MoveItem(ItemSlot.BackPack_1);

            if (slot != null)
            {
                DelayAction.Add(500, () => tpScroll.MoveItem(slot.Value));
            }
        }

        #endregion

        #region Methods

        private void BackpackOnSwap(object sender, EventArgs eventArgs)
        {
            var inventoryItems =
                items.GetMyItems(Items.StoredPlace.Inventory).Where(x => menu.Backpack.ItemEnabled(x.Name)).ToList();

            var backpackItems =
                items.GetMyItems(Items.StoredPlace.Backpack).Where(x => menu.Backpack.ItemEnabled(x.Name)).ToList();

            MoveItems(inventoryItems, backpackItems, Items.StoredPlace.Backpack);
            MoveItems(backpackItems, inventoryItems, Items.StoredPlace.Inventory);
        }

        private void MoveItem(
            Item item,
            ItemSlot slot,
            ICollection<ItemSlot> freeSlots,
            ICollection<ItemSlot> allSlots,
            Items.StoredPlace direction)
        {
            if (direction != Items.StoredPlace.Inventory)
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

        private void MoveItems(IEnumerable<Item> from, ICollection<Item> to, Items.StoredPlace direction)
        {
            var freeSlots = new List<ItemSlot>();
            var allSlots = new List<ItemSlot>();

            switch (direction)
            {
                case Items.StoredPlace.Inventory:
                    freeSlots.AddRange(hero.Inventory.FreeSlots);
                    allSlots.AddRange(items.InvenorySlots);
                    break;
                case Items.StoredPlace.Backpack:
                    freeSlots.AddRange(hero.Inventory.FreeBackpackSlots);
                    allSlots.AddRange(items.BackpackSlots);
                    break;
                case Items.StoredPlace.Stash:
                    freeSlots.AddRange(hero.Inventory.FreeStashSlots);
                    allSlots.AddRange(items.StashSlots);
                    break;
            }

            foreach (var item in from)
            {
                var swapItem = to.FirstOrDefault();

                if (swapItem != null)
                {
                    var slot = items.GetSlot(swapItem.AbilityId, direction);
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

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (!menu.Auto.SwapTpScroll || menu.Auto.TpScrollAbuse || sender.Handle != hero.Handle
                || args.Modifier.TextureName != "item_tpscroll")
            {
                return;
            }

            var backpackItem =
                items.GetMyItems(Items.StoredPlace.Backpack).OrderByDescending(x => x.Cost).FirstOrDefault();
            if (backpackItem == null)
            {
                return;
            }

            var scrollSlot = items.GetSlot(AbilityId.item_tpscroll, Items.StoredPlace.Inventory);
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
            if (item == null || item.AbilityId != AbilityId.item_cheese && item.AbilityId != AbilityId.item_aegis)
            {
                return;
            }

            var freeSlots = hero.Inventory.FreeSlots.ToList();
            if (!freeSlots.Any())
            {
                return;
            }

            var backpackItem =
                items.GetMyItems(Items.StoredPlace.Backpack).OrderByDescending(x => x.Cost).FirstOrDefault();
            backpackItem?.MoveItem(freeSlots.First());
        }

        private void StashOnSwap(object sender, EventArgs eventArgs)
        {
            if (hero.ActiveShop != ShopType.Base)
            {
                return;
            }

            var inventoryItems =
                items.GetMyItems(Items.StoredPlace.Inventory).Where(x => menu.Stash.ItemEnabled(x.Name)).ToList();

            var stashItems =
                items.GetMyItems(Items.StoredPlace.Stash).Where(x => menu.Stash.ItemEnabled(x.Name)).ToList();

            MoveItems(inventoryItems, stashItems, Items.StoredPlace.Stash);
            MoveItems(stashItems, inventoryItems, Items.StoredPlace.Inventory);
        }

        #endregion
    }
}
namespace ItemManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;

    using Menus.ItemSwap;

    internal class Items
    {
        #region Enums

        [Flags]
        public enum StoredPlace
        {
            Inventory = 1,

            Backpack = 2,

            Stash = 4,

            Any = Inventory | Backpack | Stash
        }

        #endregion

        #region Constructors and Destructors

        public Items(Hero myHero, ItemSwapMenu itemSwapper)
        {
            hero = myHero;
            menu = itemSwapper;

            foreach (
                var item in ObjectManager.GetEntities<Item>().Where(x => x.IsValid && x.Owner?.Handle == hero.Handle))
            {
                itemSwapper.AddItem(item.Name);
                items.Add(item);
            }

            ObjectManager.OnAddEntity += OnAddEntity;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
        }

        #endregion

        #region Fields

        private readonly Hero hero;

        private readonly List<Item> items = new List<Item>();

        private readonly Dictionary<Item, ItemSlot> itemSlots = new Dictionary<Item, ItemSlot>();

        private readonly ItemSwapMenu menu;

        #endregion

        #region Public Properties

        public List<ItemSlot> BackpackSlots { get; } = new List<ItemSlot>
        {
            ItemSlot.BackPack_1,
            ItemSlot.BackPack_2,
            ItemSlot.BackPack_3
        };

        public List<ItemSlot> InvenorySlots { get; } = new List<ItemSlot>
        {
            ItemSlot.InventorySlot_1,
            ItemSlot.InventorySlot_2,
            ItemSlot.InventorySlot_3,
            ItemSlot.InventorySlot_4,
            ItemSlot.InventorySlot_5,
            ItemSlot.InventorySlot_6
        };

        public List<ItemSlot> StashSlots { get; } = new List<ItemSlot>
        {
            ItemSlot.StashSlot_1,
            ItemSlot.StashSlot_2,
            ItemSlot.StashSlot_3,
            ItemSlot.StashSlot_4,
            ItemSlot.StashSlot_5,
            ItemSlot.StashSlot_6
        };

        #endregion

        #region Public Methods and Operators

        public IEnumerable<Item> GetMyItems(StoredPlace storedPlace)
        {
            var list = new List<Item>();

            if (storedPlace.HasFlag(StoredPlace.Inventory))
            {
                list.AddRange(hero.Inventory.Items);
            }

            if (storedPlace.HasFlag(StoredPlace.Backpack))
            {
                list.AddRange(hero.Inventory.Backpack);
            }

            if (storedPlace.HasFlag(StoredPlace.Stash))
            {
                list.AddRange(hero.Inventory.Stash);
            }

            return list;
        }

        public ItemSlot? GetSavedSlot(Item item)
        {
            return itemSlots.FirstOrDefault(x => x.Key.Equals(item)).Value;
        }

        public ItemSlot? GetSlot(AbilityId itemId, StoredPlace storedPlace)
        {
            var start = ItemSlot.InventorySlot_1;
            var end = ItemSlot.StashSlot_6;

            switch (storedPlace)
            {
                case StoredPlace.Inventory:
                    end = ItemSlot.InventorySlot_6;
                    break;
                case StoredPlace.Backpack:
                    start = ItemSlot.BackPack_1;
                    end = ItemSlot.BackPack_3;
                    break;
                case StoredPlace.Stash:
                    start = ItemSlot.StashSlot_1;
                    break;
            }

            for (var i = start; i <= end; i++)
            {
                var currentItem = hero.Inventory.GetItem(i);
                if (currentItem != null && currentItem.AbilityId == itemId)
                {
                    return i;
                }
            }

            return null;
        }

        public void OnClose()
        {
            ObjectManager.OnAddEntity -= OnAddEntity;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
        }

        public void SaveItemSlot(Item item)
        {
            var slot = GetSlot(item.AbilityId, StoredPlace.Any);
            if (slot != null)
            {
                itemSlots[item] = slot.Value;
            }
        }

        #endregion

        #region Methods

        private void OnAddEntity(EntityEventArgs args)
        {
            DelayAction.Add(
                500,
                () =>
                    {
                        var item = args.Entity as Item;

                        if (item == null || !item.IsValid || item.IsRecipe || args.Entity?.Owner?.Handle != hero.Handle
                            || items.Contains(item))
                        {
                            return;
                        }

                        menu.AddItem(item.Name);
                        items.Add(item);
                    });
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            var item = args.Entity as Item;

            if (item == null || !item.IsValid || args.Entity?.Owner?.Handle != hero.Handle)
            {
                return;
            }

            if (!GetMyItems(StoredPlace.Any).ToList().Exists(x => x.Name == item.Name))
            {
                menu.RemoveItem(item.Name);
            }

            items.Remove(item);
        }

        #endregion
    }
}
namespace ItemManager.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;

    using Menus;

    using Utils;

    internal class ItemManager
    {
        private readonly Hero hero;

        private readonly Dictionary<Item, ItemSlot> itemSlots = new Dictionary<Item, ItemSlot>();

        private readonly MenuManager menu;

        public ItemManager(Hero myHero, MenuManager menuManager)
        {
            hero = myHero;
            menu = menuManager;

            foreach (var item in ObjectManager.GetEntities<Item>()
                .Where(x => x.IsValid && x.Purchaser?.Hero.Handle == hero.Handle && x.Id != AbilityId.ability_base))
            {
                menu.ItemSwapMenu.AddItem(item.Name);
                AllItems.Add(item);
            }

            ObjectManager.OnAddEntity += OnAddEntity;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;
        }

        public List<Item> AllItems { get; } = new List<Item>();

        public Item GetBestBackpackItem()
        {
            return GetMyItems(ItemUtils.StoredPlace.Backpack)
                .Where(x => !x.IsRecipe && !x.IsEmptyBottle())
                .OrderByDescending(x => x.Cost)
                .FirstOrDefault();
        }

        public uint GetItemCharges(AbilityId id)
        {
            var wards = 0u;

            switch (id)
            {
                case AbilityId.item_ward_observer:
                {
                    wards = (uint)AllItems.Where(x => x.Id == AbilityId.item_ward_dispenser).Sum(x => x.CurrentCharges);
                    break;
                }
                case AbilityId.item_ward_sentry:
                {
                    wards = (uint)AllItems.Where(x => x.Id == AbilityId.item_ward_dispenser)
                        .Sum(x => x.SecondaryCharges);
                    break;
                }
            }

            return wards + (uint)AllItems.Where(x => x.Id == id).Sum(x => x.CurrentCharges);
        }

        public IEnumerable<Item> GetMyItems(ItemUtils.StoredPlace storedPlace)
        {
            var list = new List<Item>();

            if (storedPlace.HasFlag(ItemUtils.StoredPlace.Inventory))
            {
                list.AddRange(hero.Inventory.Items);
            }

            if (storedPlace.HasFlag(ItemUtils.StoredPlace.Backpack))
            {
                list.AddRange(hero.Inventory.Backpack);
            }

            if (storedPlace.HasFlag(ItemUtils.StoredPlace.Stash))
            {
                list.AddRange(hero.Inventory.Stash);
            }

            return list;
        }

        public ItemSlot? GetSavedSlot(Item item)
        {
            return itemSlots.FirstOrDefault(x => x.Key.Equals(item)).Value;
        }

        public ItemSlot? GetSlot(AbilityId abilityId, ItemUtils.StoredPlace storedPlace)
        {
            var start = ItemSlot.InventorySlot_1;
            var end = ItemSlot.StashSlot_6;

            switch (storedPlace)
            {
                case ItemUtils.StoredPlace.Inventory:
                {
                    end = ItemSlot.InventorySlot_6;
                    break;
                }
                case ItemUtils.StoredPlace.Backpack:
                {
                    start = ItemSlot.BackPack_1;
                    end = ItemSlot.BackPack_3;
                    break;
                }
                case ItemUtils.StoredPlace.Stash:
                {
                    start = ItemSlot.StashSlot_1;
                    break;
                }
            }

            for (var i = start; i <= end; i++)
            {
                var currentItem = hero.Inventory.GetItem(i);
                if (currentItem != null && currentItem.Id == abilityId)
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
            var slot = GetSlot(item.Id, ItemUtils.StoredPlace.Any);
            if (slot != null)
            {
                itemSlots[item] = slot.Value;
            }
        }

        private void OnAddEntity(EntityEventArgs args)
        {
            var item = args.Entity as Item;

            if (item == null || !item.IsValid || item.IsRecipe || item.Purchaser?.Hero.Handle != hero.Handle
                || AllItems.Contains(item) || item.Id == AbilityId.ability_base)
            {
                return;
            }

            menu.ItemSwapMenu.AddItem(item.Name);
            AllItems.Add(item);
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            var item = args.Entity as Item;

            if (item == null || !item.IsValid || item.Purchaser?.Hero.Handle != hero.Handle)
            {
                return;
            }

            if (GetMyItems(ItemUtils.StoredPlace.Any).All(x => x.Id != item.Id))
            {
                menu.ItemSwapMenu.RemoveItem(item.Name);
            }

            AllItems.Remove(item);
        }
    }
}
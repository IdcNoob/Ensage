namespace ItemManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Abilities;
    using Abilities.Base;
    using Abilities.Interfaces;

    using Attributes;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Utils;

    internal class Manager : IDisposable
    {
        private readonly HashSet<uint> addedEntities = new HashSet<uint>();

        private readonly MultiSleeper disabledItems = new MultiSleeper();

        private readonly Dictionary<Item, ItemSlot> itemSlots = new Dictionary<Item, ItemSlot>();

        private readonly List<Type> types;

        public Manager()
        {
            MyHero = ObjectManager.LocalHero;
            MyHandle = MyHero.Handle;
            MyTeam = MyHero.Team;

            types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.Namespace == "ItemManager.Core.Abilities")
                .ToList();

            ObjectManager.OnAddEntity += OnAddEntity;
            ObjectManager.OnRemoveEntity += OnRemoveEntity;

            DelayAction.Add(3000, AddCurrentObjects);
        }

        public event EventHandler<AbilityEventArgs> OnAbilityAdd;

        public event EventHandler<AbilityEventArgs> OnAbilityRemove;

        public event EventHandler<ItemEventArgs> OnItemAdd;

        public event EventHandler<ItemEventArgs> OnItemRemove;

        public event EventHandler<UnitEventArgs> OnUnitAdd;

        public event EventHandler<UnitEventArgs> OnUnitRemove;

        public List<Item> DroppedItems { get; } = new List<Item>();

        public List<Ability> MyAbilities { get; } = new List<Ability>();

        public uint MyHandle { get; }

        public float MyHealthPercentage => (float)MyHero.Health / MyHero.MaximumHealth * 100;

        public Hero MyHero { get; }

        public List<Item> MyItems { get; } = new List<Item>();

        public float MyManaPercentage => MyHero.Mana / MyHero.MaximumMana * 100;

        public float MyMissingHealth => (float)MyHero.MaximumHealth - MyHero.Health;

        public float MyMissingMana => MyHero.MaximumMana - MyHero.Mana;

        public Team MyTeam { get; }

        //public List<Unit> Units { get; } = new List<Unit>();

        public List<UsableAbility> UsableAbilities { get; } = new List<UsableAbility>();

        public void Dispose()
        {
            ObjectManager.OnAddEntity -= OnAddEntity;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;

            MyAbilities.Clear();
            MyItems.Clear();
            addedEntities.Clear();
            //Units.Clear();
            types.Clear();
            DroppedItems.Clear();
        }

        public void DropItem(Item item, ItemUtils.StoredPlace stored = ItemUtils.StoredPlace.Any, bool saveSlot = true)
        {
            if (saveSlot)
            {
                SaveItemSlot(item, stored);
            }

            MyHero.DropItem(item, MyHero.Position, true);
            DroppedItems.Add(item);
        }

        public void DropItems(
            ItemUtils.Stats dropStats,
            bool toBackpack = false,
            params IRecoveryAbility[] ignoredItems)
        {
            foreach (var item in GetMyItems(ItemUtils.StoredPlace.Inventory)
                .Where(
                    x => ignoredItems.All(z => z.Handle != x.Handle) && !DroppedItems.Contains(x)
                         && !disabledItems.Sleeping(x.Handle) && x.IsEnabled && x.IsDroppable
                         && x.GetItemStats().HasFlag(dropStats)))
            {
                if (toBackpack && MyHero.ItemsCanBeDisabled())
                {
                    var slot = GetSlot(item.Handle, ItemUtils.StoredPlace.Inventory);
                    item.MoveItem(ItemSlot.BackPack_1);
                    disabledItems.Sleep(6000, item.Handle);
                    UsableAbilities.FirstOrDefault(x => x.Handle == item.Handle)?.SetSleep(6000);

                    if (slot != null)
                    {
                        item.MoveItem(slot.Value);
                    }
                }
                else
                {
                    DropItem(item, ItemUtils.StoredPlace.Inventory);
                }
            }
        }

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
                    wards = (uint)MyItems.Where(x => x.Id == AbilityId.item_ward_dispenser).Sum(x => x.CurrentCharges);
                    break;
                }
                case AbilityId.item_ward_sentry:
                {
                    wards = (uint)MyItems.Where(x => x.Id == AbilityId.item_ward_dispenser)
                        .Sum(x => x.SecondaryCharges);
                    break;
                }
            }

            return wards + (uint)MyItems.Where(x => x.Id == id).Sum(x => x.CurrentCharges);
        }

        public IEnumerable<Item> GetMyItems(ItemUtils.StoredPlace storedPlace)
        {
            var list = new List<Item>();

            if (storedPlace.HasFlag(ItemUtils.StoredPlace.Inventory))
            {
                list.AddRange(MyHero.Inventory.Items);
            }

            if (storedPlace.HasFlag(ItemUtils.StoredPlace.Backpack))
            {
                list.AddRange(MyHero.Inventory.Backpack);
            }

            if (storedPlace.HasFlag(ItemUtils.StoredPlace.Stash))
            {
                list.AddRange(MyHero.Inventory.Stash);
            }

            return list;
        }

        public ItemSlot? GetSavedSlot(Item item)
        {
            return itemSlots.FirstOrDefault(x => x.Key.Equals(item)).Value;
        }

        public ItemSlot? GetSavedSlot(uint handle)
        {
            return itemSlots.FirstOrDefault(x => x.Key.Handle == handle).Value;
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
                var currentItem = MyHero.Inventory.GetItem(i);
                if (currentItem != null && currentItem.Id == abilityId)
                {
                    return i;
                }
            }

            return null;
        }

        public ItemSlot? GetSlot(uint handle, ItemUtils.StoredPlace storedPlace)
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
                var currentItem = MyHero.Inventory.GetItem(i);
                if (currentItem != null && currentItem.Handle == handle)
                {
                    return i;
                }
            }

            return null;
        }

        public bool MyHeroCanUseItems()
        {
            return MyHero.IsAlive && MyHero.CanUseItems() && !MyHero.IsChanneling() && !MyHero.IsInvisible();
        }

        public float PickUpItems()
        {
            var bottle = UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_bottle) as Bottle;
            if (bottle != null && bottle.TookFromStash)
            {
                var slot = GetSavedSlot(bottle.Handle);
                if (slot != null)
                {
                    bottle.MoveItem(slot.Value, false);
                }
            }

            if (!DroppedItems.Any(x => x != null && x.IsValid && x.IsVisible))
            {
                return 0;
            }

            var items = ObjectManager.GetEntitiesParallel<PhysicalItem>()
                .Where(x => x.IsVisible && x.Distance2D(MyHero) < 800 && DroppedItems.Contains(x.Item))
                .Reverse();

            if (!items.Any())
            {
                return 0;
            }

            MyHero.Stop();

            var sleep = DroppedItems.Count * Game.Ping;

            foreach (var physicalItem in items)
            {
                if (MyHero.PickUpItem(physicalItem, true))
                {
                    DroppedItems.Remove(physicalItem.Item);

                    var slot = GetSavedSlot(physicalItem.Item);
                    var item = MyItems.FirstOrDefault(x => x.Handle == physicalItem.Item.Handle);

                    if (slot != null && item != null)
                    {
                        DelayAction.Add(200, () => { item.MoveItem(slot.Value); });
                    }
                }
            }

            return sleep;
        }

        public void SaveItemSlot(Item item, ItemUtils.StoredPlace stored = ItemUtils.StoredPlace.Any)
        {
            var slot = GetSlot(item.Handle, stored);
            if (slot != null)
            {
                itemSlots[item] = slot.Value;
            }
        }

        private void AddCurrentObjects()
        {
            foreach (var hero in ObjectManager.GetEntities<Player>().Where(x => x?.Hero != null).Select(x => x.Hero))
            {
                OnAddEntity(new EntityEventArgs(hero));

                var abilities = new List<Ability>();

                abilities.AddRange(hero.Spellbook.Spells);
                abilities.AddRange(hero.Inventory.Items);
                abilities.AddRange(hero.Inventory.Stash);
                abilities.AddRange(hero.Inventory.Backpack);

                foreach (var ability in abilities)
                {
                    OnAddEntity(new EntityEventArgs(ability));
                }
            }

            foreach (var unit in ObjectManager.GetEntities<Unit>())
            {
                OnAddEntity(new EntityEventArgs(unit));
            }
        }

        private void OnAddEntity(EntityEventArgs args)
        {
            if (addedEntities.Contains(args.Entity.Handle))
            {
                return;
            }

            addedEntities.Add(args.Entity.Handle);

            var ability = args.Entity as Ability;
            if (ability != null && ability.IsValid && ability.Id != AbilityId.ability_base)
            {
                var isMine = ability.Owner?.Handle == MyHero.Handle;
                if (isMine)
                {
                    MyAbilities.Add(ability);

                    var type = types.FirstOrDefault(
                        x => x.GetCustomAttributes<AbilityAttribute>().Any(z => z.AbilityId == ability.Id));

                    if (type != null)
                    {
                        UsableAbilities.Add((UsableAbility)Activator.CreateInstance(type, ability, this));
                    }
                }

                OnAbilityAdd?.Invoke(null, new AbilityEventArgs(ability, isMine));
            }

            var item = args.Entity as Item;
            if (item != null && item.IsValid && item.Id != AbilityId.ability_base)
            {
                var isMine = item.Purchaser?.Hero?.Handle == MyHero.Handle;
                if (isMine)
                {
                    MyItems.Add(item);
                }

                OnItemAdd?.Invoke(null, new ItemEventArgs(item, isMine));
                return;
            }

            var unit = args.Entity as Unit;
            if (unit != null && unit.IsValid)
            {
                // Units.Add(unit);
                OnUnitAdd?.Invoke(null, new UnitEventArgs(unit));
            }
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            addedEntities.Remove(args.Entity.Handle);

            var physItem = args.Entity as PhysicalItem;
            if (physItem != null && physItem.IsValid && physItem.Item?.Purchaser?.Hero?.Handle == MyHero.Handle)
            {
                DroppedItems.Remove(physItem.Item);
                return;
            }

            var ability = args.Entity as Ability;
            if (ability != null && ability.IsValid)
            {
                var isMine = ability.Owner?.Handle == MyHero.Handle;
                if (isMine)
                {
                    MyAbilities.Remove(ability);
                    var usableAbility = UsableAbilities.FirstOrDefault(x => x.Handle == ability.Handle);
                    if (usableAbility != null)
                    {
                        UsableAbilities.Remove(usableAbility);
                    }
                }

                OnAbilityRemove?.Invoke(null, new AbilityEventArgs(ability, isMine));
            }

            var item = args.Entity as Item;
            if (item != null && item.IsValid)
            {
                var isMine = item.Purchaser?.Hero?.Handle == MyHero.Handle;
                if (isMine)
                {
                    MyItems.Remove(item);
                }

                OnItemRemove?.Invoke(null, new ItemEventArgs(item, isMine));
                return;
            }

            var unit = args.Entity as Unit;
            if (unit != null && unit.IsValid)
            {
                //Units.Remove(unit);
                OnUnitRemove?.Invoke(null, new UnitEventArgs(unit));
            }
        }
    }
}
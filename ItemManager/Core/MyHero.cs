namespace ItemManager.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;
    using Abilities.Base;
    using Abilities.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Helpers;

    using SharpDX;

    using Utils;

    internal class MyHero : IDisposable
    {
        private readonly List<Ability> abilities = new List<Ability>();

        private readonly MultiSleeper disabledItems = new MultiSleeper();

        private readonly HashSet<OrderId> dropTargetOrders = new HashSet<OrderId>
        {
            OrderId.Hold,
            OrderId.Stop,
            OrderId.MoveLocation,
            OrderId.MoveTarget
        };

        private readonly List<Item> items = new List<Item>();

        private readonly Dictionary<Item, ItemSlot> itemSlots = new Dictionary<Item, ItemSlot>();

        public MyHero(Hero hero)
        {
            Hero = hero;
            Player = hero.Player;
            Handle = hero.Handle;
            Team = hero.Team;
            EnemyTeam = hero.GetEnemyTeam();
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public IEnumerable<Ability> Abilities => abilities.Where(x => x.IsValid);

        public float Damage => Hero.MinimumDamage + Hero.BonusDamage;

        public List<Item> DroppedItems { get; } = new List<Item>();

        public Team EnemyTeam { get; }

        public uint Handle { get; }

        public float Health => Hero.Health;

        public float HealthPercentage => (Health / MaximumHealth) * 100;

        public Hero Hero { get; }

        public Inventory Inventory => Hero.Inventory;

        public bool IsAlive => Hero.IsAlive;

        public bool IsChanneling => Hero.IsChanneling();

        public IEnumerable<Item> Items => items.Where(x => x.IsValid);

        public uint Level => Hero.Level;

        public float Mana => Hero.Mana;

        public float ManaPercentage => (Mana / MaximumMana) * 100;

        public float MaximumHealth => Hero.MaximumHealth;

        public float MaximumMana => Hero.MaximumMana;

        public float MissingHealth => MaximumHealth - Health;

        public float MissingMana => MaximumMana - Mana;

        public Player Player { get; }

        public Vector3 Position => Hero.Position;

        public Unit Target { get; private set; }

        public Team Team { get; }

        public List<UsableAbility> UsableAbilities { get; } = new List<UsableAbility>();

        public void AddAbility(Ability ability)
        {
            abilities.Add(ability);
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public bool CanAttack()
        {
            return IsAlive && !IsChanneling && Hero.CanAttack();
        }

        public bool CanUseAbilities()
        {
            return CanUse() && Hero.CanCast();
        }

        public bool CanUseAbilitiesInInvisibility()
        {
            return Hero.HasModifiers(ModifierUtils.CanUseAbilitiesInInvis.ToArray(), false);
        }

        public bool CanUseItems()
        {
            return CanUse() && Hero.CanUseItems();
        }

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public float Distance2D(Entity entity)
        {
            return Hero.Distance2D(entity);
        }

        public float Distance2D(Vector3 position)
        {
            return Hero.Distance2D(position);
        }

        public void DropItem(Item item, ItemStoredPlace itemStored = ItemStoredPlace.Any, bool saveSlot = true)
        {
            if (saveSlot)
            {
                SaveItemSlot(item, itemStored);
            }

            Hero.DropItem(item, Hero.Position, true);
            DroppedItems.Add(item);
        }

        public void DropItems(ItemStats dropItemStats, bool toBackpack = false, params IRecoveryAbility[] ignoredItems)
        {
            foreach (var item in GetItems(ItemStoredPlace.Inventory)
                .Where(
                    x => ignoredItems.All(z => z.Handle != x.Handle) && !DroppedItems.Contains(x) && !disabledItems.Sleeping(x.Handle)
                         && x.IsEnabled && x.IsDroppable && (x.GetItemStats() & dropItemStats) != 0))
            {
                if (toBackpack && ItemsCanBeDisabled())
                {
                    if (!item.CanBeMovedToBackpack())
                    {
                        continue;
                    }

                    var slot = GetItemSlot(item.Handle, ItemStoredPlace.Inventory);
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
                    DropItem(item, ItemStoredPlace.Inventory);
                }
            }
        }

        public Item GetBestBackpackItem()
        {
            return GetItems(ItemStoredPlace.Backpack)
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
                    wards = (uint)Items.Where(x => x.Id == AbilityId.item_ward_dispenser).Sum(x => x.CurrentCharges);
                    break;
                }
                case AbilityId.item_ward_sentry:
                {
                    wards = (uint)Items.Where(x => x.Id == AbilityId.item_ward_dispenser).Sum(x => x.SecondaryCharges);
                    break;
                }
            }

            return wards + (uint)Items.Where(x => x.Id == id).Sum(x => x.CurrentCharges);
        }

        public IEnumerable<Item> GetItems(ItemStoredPlace itemStoredPlace)
        {
            var list = new List<Item>();

            if (itemStoredPlace.HasFlag(ItemStoredPlace.Inventory))
            {
                list.AddRange(Hero.Inventory.Items);
            }

            if (itemStoredPlace.HasFlag(ItemStoredPlace.Backpack))
            {
                list.AddRange(Hero.Inventory.Backpack);
            }

            if (itemStoredPlace.HasFlag(ItemStoredPlace.Stash))
            {
                list.AddRange(Hero.Inventory.Stash);
            }

            return list;
        }

        public ItemSlot? GetItemSlot(AbilityId abilityId, ItemStoredPlace itemStoredPlace)
        {
            return GetItemSlot(null, abilityId, itemStoredPlace);
        }

        public ItemSlot? GetItemSlot(uint handle, ItemStoredPlace itemStoredPlace)
        {
            return GetItemSlot(handle, null, itemStoredPlace);
        }

        public ItemSlot? GetSavedItemSlot(Item item)
        {
            return itemSlots.FirstOrDefault(x => x.Key.Equals(item)).Value;
        }

        public ItemSlot? GetSavedItemSlot(uint handle)
        {
            return itemSlots.FirstOrDefault(x => x.Key.Handle == handle).Value;
        }

        public bool HasModifier(string modifierName)
        {
            return Hero.HasModifier(modifierName);
        }

        public bool IsAtBase()
        {
            return Hero.ActiveShop == ShopType.Base;
        }

        public bool IsInvisible()
        {
            return Hero.IsInvisible() && !Hero.IsVisibleToEnemies;
        }

        public bool ItemsCanBeDisabled()
        {
            return Hero.ActiveShop == ShopType.None;
        }

        public float PickUpItems()
        {
            var bottle = UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_bottle) as Bottle;
            if (bottle != null && bottle.TakenFromStash)
            {
                var slot = GetSavedItemSlot(bottle.Handle);
                if (slot != null)
                {
                    bottle.MoveItem(slot.Value, false);
                }
            }

            if (!DroppedItems.Any(x => x != null && x.IsValid && x.IsVisible))
            {
                return 0;
            }

            var physicalItems = EntityManager<PhysicalItem>.Entities
                .Where(x => x.IsValid && x.IsVisible && x.Distance2D(Hero) < 800 && DroppedItems.Contains(x.Item))
                .Reverse()
                .ToList();

            if (!physicalItems.Any())
            {
                return 0;
            }

            Hero.Stop();

            var sleep = DroppedItems.Count * 50;

            foreach (var physicalItem in physicalItems)
            {
                if (Hero.PickUpItem(physicalItem, true))
                {
                    DroppedItems.Remove(physicalItem.Item);

                    var slot = GetSavedItemSlot(physicalItem.Item);
                    var item = Items.FirstOrDefault(x => x.Handle == physicalItem.Item.Handle);

                    if (slot != null && item != null)
                    {
                        UpdateManager.BeginInvoke(() => item.MoveItem(slot.Value), 200);
                    }
                }
            }

            return sleep;
        }

        public void RemoveAbility(Ability ability)
        {
            abilities.Remove(ability);
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public void SaveItemSlot(Item item, ItemStoredPlace itemStored = ItemStoredPlace.Any)
        {
            var slot = GetItemSlot(item.Handle, itemStored);
            if (slot != null)
            {
                itemSlots[item] = slot.Value;
            }
        }

        private bool CanUse()
        {
            return Hero.IsAlive && !IsChanneling && (!IsInvisible() || CanUseAbilitiesInInvisibility());
        }

        private ItemSlot? GetItemSlot(uint? handle, AbilityId? abilityId, ItemStoredPlace itemStoredPlace)
        {
            var start = ItemSlot.InventorySlot_1;
            var end = ItemSlot.StashSlot_6;

            switch (itemStoredPlace)
            {
                case ItemStoredPlace.Inventory:
                {
                    end = ItemSlot.InventorySlot_6;
                    break;
                }
                case ItemStoredPlace.Backpack:
                {
                    start = ItemSlot.BackPack_1;
                    end = ItemSlot.BackPack_3;
                    break;
                }
                case ItemStoredPlace.Stash:
                {
                    start = ItemSlot.StashSlot_1;
                    break;
                }
            }

            for (var i = start; i <= end; i++)
            {
                var currentItem = Inventory.GetItem(i);
                if (currentItem != null && (abilityId != null && currentItem.Id == abilityId.Value
                                            || handle != null && currentItem.Handle == handle.Value))
                {
                    return i;
                }
            }

            return null;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(Hero) || !args.Process)
            {
                return;
            }

            if (args.OrderId == OrderId.AttackTarget)
            {
                var target = args.Target as Hero;
                if (target != null && target.IsAlive && !target.IsIllusion && target.Team != Team)
                {
                    Target = target;
                }
            }
            else if (dropTargetOrders.Contains(args.OrderId))
            {
                Target = null;
            }
        }
    }
}
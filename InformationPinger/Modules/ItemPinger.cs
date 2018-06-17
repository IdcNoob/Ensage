namespace InformationPinger.Modules
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.Items;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Service;

    using Interfaces;

    using PingTypes;

    [Export(typeof(IModule))]
    internal class ItemPinger : IModule
    {
        private readonly Team enemyTeam;

        private readonly IInformationPinger informationPinger;

        private readonly Unit myHero;

        private readonly IMenuManager rootMenu;

        private MenuItem<AbilityToggler> bottledRune;

        private MenuItem<bool> doublePing;

        private MenuItem<bool> enabled;

        private MenuItem<bool> enemyCheck;

        private MenuItem<AbilityToggler> forcePingItems;

        private MenuItem<Slider> itemCostThreshold;

        private HashSet<uint> pingedItems;

        private bool processing;

        private Queue<Item> queue;

        [ImportingConstructor]
        public ItemPinger(
            [Import] IServiceContext context,
            [Import] IMenuManager menu,
            [Import] IInformationPinger pinger)
        {
            myHero = context.Owner;
            enemyTeam = myHero.GetEnemyTeam();
            rootMenu = menu;
            informationPinger = pinger;
        }

        public bool IsActive { get; private set; }

        public void Activate()
        {
            CreateMenu();

            queue = new Queue<Item>();
            pingedItems = new HashSet<uint>();

            if (enabled)
            {
                EntityManager<Item>.EntityAdded += OnEntityAdded;
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            }
            enabled.Item.ValueChanged += ItemOnValueChanged;
        }

        public void Dispose()
        {
            EntityManager<Item>.EntityAdded -= OnEntityAdded;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            enabled.Item.ValueChanged -= ItemOnValueChanged;
        }

        private void AddPingItem(Item item)
        {
            queue.Enqueue(item);

            if (processing)
            {
                return;
            }

            processing = true;
            UpdateManager.BeginInvoke(ProceedQueue);
        }

        private void CreateMenu()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            var menu = rootMenu.MenuFactory.Menu("Items");
            enabled = menu.Item("Enabled", true);
            enabled.Item.SetTooltip("Ping enemy items");
            itemCostThreshold = menu.Item("Item cost", new Slider(1800, 99, 5000));
            itemCostThreshold.Item.SetTooltip("Will ping items that costs more");
            doublePing = menu.Item("Double ping", false);
            doublePing.Item.SetTooltip("Will ping items 2 times");
            forcePingItems = menu.Item(
                "Force ping:",
                new AbilityToggler(
                    new Dictionary<string, bool>
                    {
                        { "item_smoke_of_deceit", true },
                        { "item_dust", true },
                        { "item_gem", true },
                        { "item_ward_dispenser", true },
                        { "item_ward_sentry", true },
                        { "item_ward_observer", true }
                    }));

            bottledRune = menu.Item(
                "Bottled rune ping:",
                new AbilityToggler(
                    new Dictionary<string, bool>
                    {
                        { "item_bottle_illusion", false },
                        { "item_bottle_regeneration", true },
                        { "item_bottle_arcane", true },
                        { "item_bottle_invisibility", true },
                        { "item_bottle_doubledamage", true },
                        { "item_bottle_haste", true }
                    }));

            enemyCheck = menu.Item("Check enemies", false);
            enemyCheck.Item.SetTooltip("If there is any enemy hero/creep near you it won't ping");
        }

        private bool IsRunePingEnabledFor(RuneType rune)
        {
            switch (rune)
            {
                case RuneType.DoubleDamage:
                    return bottledRune.Value.IsEnabled("item_bottle_doubledamage");
                case RuneType.Haste:
                    return bottledRune.Value.IsEnabled("item_bottle_haste");
                case RuneType.Illusion:
                    return bottledRune.Value.IsEnabled("item_bottle_illusion");
                case RuneType.Invisibility:
                    return bottledRune.Value.IsEnabled("item_bottle_invisibility");
                case RuneType.Regeneration:
                    return bottledRune.Value.IsEnabled("item_bottle_regeneration");
                case RuneType.Arcane:
                    return bottledRune.Value.IsEnabled("item_bottle_arcane");
                default:
                    return false;
            }
        }

        private void ItemOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                EntityManager<Item>.EntityAdded += OnEntityAdded;
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            }
            else
            {
                EntityManager<Item>.EntityAdded -= OnEntityAdded;
                Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            }
        }

        private bool ItemPingCheck()
        {
            if (!enemyCheck || !myHero.IsAlive)
            {
                return true;
            }

            return !EntityManager<Unit>.Entities.Any(
                       x => x.IsValid && x.IsAlive && x.IsRealUnit() && x.Team == enemyTeam
                            && x.Distance2D(myHero) <= 700);
        }

        private void OnEntityAdded(object sender, Item item)
        {
            if (!item.IsValid || pingedItems.Contains(item.Handle)
                || item.Cost < itemCostThreshold && !forcePingItems.Value.IsEnabled(item.Name))
            {
                return;
            }

            var owner = item.Owner as Hero;
            if (owner == null || !owner.IsValid || owner.IsIllusion || owner.Team != enemyTeam)
            {
                return;
            }

            pingedItems.Add(item.Handle);
            AddPingItem(item);
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.NewValue == args.OldValue || args.PropertyName != "m_iStoredRuneType")
            {
                return;
            }

            var bottle = sender as Bottle;
            if (bottle == null || !bottle.IsValid)
            {
                return;
            }

            var owner = bottle.Owner as Hero;
            if (owner == null || !owner.IsValid || owner.Team != enemyTeam)
            {
                return;
            }

            if (IsRunePingEnabledFor((RuneType)args.NewValue))
            {
                AddPingItem(bottle);
            }
        }

        private async void ProceedQueue()
        {
            while (queue.Any())
            {
                while (!ItemPingCheck())
                {
                    await Task.Delay(300);
                }

                informationPinger.AddPing(new AbilityPing(queue.Dequeue(), doublePing));

                await Task.Delay(1000);
            }

            processing = false;
        }
    }
}
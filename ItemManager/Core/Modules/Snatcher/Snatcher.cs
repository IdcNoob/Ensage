namespace ItemManager.Core.Modules.Snatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Controllables;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Menus;
    using Menus.Modules.Snatcher;

    using SharpDX;

    using Utils;

    using Hero = Controllables.Hero;

    [Module]
    internal class Snatcher : IDisposable
    {
        private readonly List<Controllable> controllables = new List<Controllable>();

        private readonly List<uint> ignoredItems = new List<uint>();

        private readonly Manager manager;

        private readonly SnatcherMenu menu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private readonly IUpdateHandler updateHandler;

        public Snatcher(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.SnatcherMenu;

            controllables.Add(new Hero(manager.MyHero.Hero));

            if (this.menu.UseOtherUnits)
            {
                AddOtherUnits();

                manager.OnUnitRemove += OnUnitRemove;
                manager.OnUnitAdd += OnUnitAdd;
            }

            updateHandler = UpdateManager.Subscribe(OnUpdate, this.menu.UpdateRate, this.menu.IsEnabled);
            Player.OnExecuteOrder += OnExecuteOrder;
            this.menu.OnUseOtherUnitsChange += MenuOnUseOtherUnitsChange;
            this.menu.OnUpdateRateChange += MenuOnUpdateRateChange;
            this.menu.OnEnabledChange += MenuOnEnabledChange;
            this.menu.OnNotificationEnabledChange += MenuOnNotificationEnabledChange;
            this.menu.OnDebug += MenuOnDebug;
            this.menu.OnFastSnatch += MenuOnOnFastSnatch;
            if (this.menu.IsNotificationEnabled && this.menu.IsEnabled)
            {
                Drawing.OnDraw += OnDraw;
            }
        }

        public void Dispose()
        {
            menu.OnDebug -= MenuOnDebug;
            menu.OnEnabledChange -= MenuOnEnabledChange;
            menu.OnUseOtherUnitsChange -= MenuOnUseOtherUnitsChange;
            menu.OnUpdateRateChange -= MenuOnUpdateRateChange;
            menu.OnFastSnatch += MenuOnOnFastSnatch;
            UpdateManager.Unsubscribe(OnUpdate);
            Player.OnExecuteOrder -= OnExecuteOrder;
            manager.OnUnitAdd -= OnUnitAdd;
            manager.OnUnitRemove -= OnUnitRemove;
            Drawing.OnDraw -= OnDraw;
            Game.OnUpdate -= FastSnatchKappa;
        }

        private void AddOtherUnits()
        {
            var controllableUnits = EntityManager<Unit>.Entities
                .Where(x => x.IsValid && x.Team == manager.MyHero.Team && x.IsControllable && !x.IsIllusion)
                .ToList();

            var spiritBear = controllableUnits.FirstOrDefault(x => x.Name.Contains("npc_dota_lone_druid_bear"));
            if (spiritBear != null)
            {
                controllables.Add(new SpiritBear(spiritBear));
            }

            foreach (var meepo in controllableUnits.Where(x => x.Name == "npc_dota_hero_meepo" && x.Handle != manager.MyHero.Handle))
            {
                controllables.Add(new MeepoClone(meepo));
            }
        }

        private void FastSnatchKappa(EventArgs args)
        {
            var hero = manager.MyHero.Hero;
            var rune = ObjectManager.GetEntities<Rune>().FirstOrDefault(x => x.Distance2D(hero) < 400);

            if (rune == null)
            {
                return;
            }

            hero.PickUpRune(rune);
            Game.OnUpdate -= FastSnatchKappa;
        }

        private void MenuOnDebug(object sender, EventArgs eventArgs)
        {
            Console.WriteLine();
            Console.WriteLine("[Snatcher info] >>>>>>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine();
            Console.WriteLine("controlables: ");
            foreach (var controllable in controllables)
            {
                var handle = controllable.Handle;
                var unit = EntityManager<Unit>.Entities.FirstOrDefault(x => x.Handle == handle);
                Console.WriteLine(" > " + unit?.Name + " // is valid: " + controllable.IsValid());
            }
            Console.WriteLine();
            Console.WriteLine("runes: ");
            foreach (var rune in EntityManager<Rune>.Entities.Where(x => x.IsVisible))
            {
                Console.WriteLine(" > " + rune.RuneType);
                foreach (var controllable in controllables.Where(x => x.IsValid()))
                {
                    if (controllable.CanPick(rune))
                    {
                        var handle = controllable.Handle;
                        var unit = EntityManager<Unit>.Entities.FirstOrDefault(x => x.Handle == handle);
                        Console.WriteLine(" >> " + unit?.Name);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            Console.WriteLine();
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                menu.OnFastSnatch += MenuOnOnFastSnatch;
                updateHandler.IsEnabled = true;
                if (menu.IsNotificationEnabled)
                {
                    Drawing.OnDraw += OnDraw;
                }
            }
            else
            {
                menu.OnFastSnatch -= MenuOnOnFastSnatch;
                updateHandler.IsEnabled = false;
                Drawing.OnDraw -= OnDraw;
            }
        }

        private void MenuOnNotificationEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled && menu.IsEnabled)
            {
                Drawing.OnDraw += OnDraw;
            }
            else
            {
                Drawing.OnDraw -= OnDraw;
            }
        }

        private void MenuOnOnFastSnatch(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                Game.OnUpdate += FastSnatchKappa;
            }
            else
            {
                Game.OnUpdate -= FastSnatchKappa;
            }
        }

        private void MenuOnUpdateRateChange(object sender, IntEventArgs intEventArgs)
        {
            updateHandler.SetUpdateRate(intEventArgs.Time);
        }

        private void MenuOnUseOtherUnitsChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                AddOtherUnits();
                manager.OnUnitAdd += OnUnitAdd;
                manager.OnUnitRemove += OnUnitRemove;
            }
            else
            {
                controllables.RemoveAll(x => x.Handle != manager.MyHero.Handle);
                manager.OnUnitAdd -= OnUnitAdd;
                manager.OnUnitRemove -= OnUnitRemove;
            }
        }

        private void OnDraw(EventArgs args)
        {
            if ((!menu.NotificationHold || !menu.HoldKey) && (!menu.NotificationToggle || !menu.ToggleKey))
            {
                return;
            }

            Drawing.DrawText(
                "Snatcher",
                "Arial",
                new Vector2(menu.NotificationX, menu.NotificationY),
                new Vector2(menu.NotificationSize),
                Color.Orange,
                FontFlags.None);
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.IsPlayerInput || !args.Process)
            {
                return;
            }

            if (args.OrderId == OrderId.DropItem)
            {
                ignoredItems.Add(args.Ability.Handle);
            }
            else if (args.OrderId == OrderId.PickItem)
            {
                var physicalItem = args.Target as PhysicalItem;
                if (physicalItem != null)
                {
                    ignoredItems.RemoveAll(x => x == physicalItem.Item.Handle);
                }
            }
        }

        private void OnUnitAdd(object sender, UnitEventArgs unitEventArgs)
        {
            var unit = unitEventArgs.Unit;
            if (unit.IsIllusion || unit.Team != manager.MyHero.Team || !unit.IsControllable
                || controllables.Any(x => x.Handle == unit.Handle))
            {
                return;
            }

            switch (unit.Name)
            {
                case "npc_dota_lone_druid_bear1":
                case "npc_dota_lone_druid_bear2":
                case "npc_dota_lone_druid_bear3":
                case "npc_dota_lone_druid_bear4":
                {
                    controllables.Add(new SpiritBear(unit));
                    break;
                }
                case "npc_dota_hero_meepo":
                {
                    controllables.Add(new MeepoClone(unit));
                    break;
                }
            }
        }

        private void OnUnitRemove(object sender, UnitEventArgs unitEventArgs)
        {
            var controllable = controllables.FirstOrDefault(x => x.Handle == unitEventArgs.Unit.Handle);
            if (controllable != null)
            {
                controllables.Remove(controllable);
            }
        }

        private void OnUpdate()
        {
            if (Game.IsPaused)
            {
                return;
            }

            var validControllables = controllables.Where(x => x.IsValid()).ToList();

            if (menu.ToggleKey && menu.EnabledToggleItems.Contains(0) || menu.HoldKey && menu.EnabledHoldItems.Contains(0))
            {
                //     var runes = EntityManager<Rune>.Entities.Where(x => x.IsVisible && !sleeper.Sleeping(x.Handle));
                var runes = ObjectManager.GetEntities<Rune>().Where(x => !sleeper.Sleeping(x.Handle));

                foreach (var rune in runes)
                {
                    foreach (var controllable in validControllables)
                    {
                        if (controllable.CanPick(rune))
                        {
                            controllable.Pick(rune);
                            sleeper.Sleep(500, rune.Handle);
                            break;
                        }
                    }
                }
            }

            var items = EntityManager<PhysicalItem>.Entities.Where(
                    x => x.IsValid && x.IsVisible && !ignoredItems.Contains(x.Item.Handle) && !sleeper.Sleeping(x.Handle)
                         && (menu.ToggleKey && menu.EnabledToggleItems.Contains(x.Item.Id)
                             || menu.HoldKey && menu.EnabledHoldItems.Contains(x.Item.Id)))
                .OrderByDescending(x => x.Item.Id == AbilityId.item_aegis);

            foreach (var item in items)
            {
                foreach (var controllable in validControllables)
                {
                    if (controllable.CanPick(item, menu.ItemMoveCostThreshold))
                    {
                        controllable.Pick(item);
                        sleeper.Sleep(500, item.Handle);
                        break;
                    }
                }
            }
        }
    }
}
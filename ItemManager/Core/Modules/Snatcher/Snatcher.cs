namespace ItemManager.Core.Modules.Snatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Controllables;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;
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
            if (this.menu.IsNotificationEnabled && this.menu.IsEnabled)
            {
                Drawing.OnDraw += OnDraw;
            }
        }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            menu.OnUseOtherUnitsChange -= MenuOnUseOtherUnitsChange;
            menu.OnUpdateRateChange -= MenuOnUpdateRateChange;
            UpdateManager.Unsubscribe(OnUpdate);
            Player.OnExecuteOrder -= OnExecuteOrder;
            manager.OnUnitAdd -= OnUnitAdd;
            manager.OnUnitRemove -= OnUnitRemove;
            Drawing.OnDraw -= OnDraw;
        }

        private void AddOtherUnits()
        {
            var controllableUnits = EntityManager<Unit>.Entities
                .Where(x => x.IsValid && x.Team == manager.MyHero.Team && x.IsControllable && !x.IsIllusion)
                .ToList();

            var spiritBear = controllableUnits.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Unit_SpiritBear);
            if (spiritBear != null)
            {
                controllables.Add(new SpiritBear(spiritBear));
            }

            foreach (var meepo in controllableUnits.Where(
                x => x.ClassId == ClassId.CDOTA_Unit_Hero_Meepo && x.Handle != manager.MyHero.Handle))
            {
                controllables.Add(new MeepoClone(meepo));
            }
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                updateHandler.IsEnabled = true;
                if (menu.IsNotificationEnabled)
                {
                    Drawing.OnDraw += OnDraw;
                }
            }
            else
            {
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
            if (unit.IsIllusion || !unit.IsControllable || controllables.Any(x => x.Handle == unit.Handle))
            {
                return;
            }

            switch (unit.ClassId)
            {
                case ClassId.CDOTA_Unit_SpiritBear:
                {
                    controllables.Add(new SpiritBear(unit));
                    break;
                }
                case ClassId.CDOTA_Unit_Hero_Meepo:
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

            if (menu.ToggleKey && menu.EnabledToggleItems.Contains(0)
                || menu.HoldKey && menu.EnabledHoldItems.Contains(0))
            {
                //var runes = EntityManager<Rune>.Entities
                var runes = ObjectManager.GetEntitiesParallel<Rune>()
                    .Where(x => !sleeper.Sleeping(x.Handle) && x.IsVisible);

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
                x => x.IsVisible && !ignoredItems.Contains(x.Item.Handle) && !sleeper.Sleeping(x.Handle)
                     && (menu.ToggleKey && menu.EnabledToggleItems.Contains(x.Item.Id)
                         || menu.HoldKey && menu.EnabledHoldItems.Contains(x.Item.Id)));

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
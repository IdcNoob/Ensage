namespace ItemManager.Core.Modules.Snatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Controllables;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus;
    using Menus.Modules.Snatcher;

    [Module]
    internal class Snatcher : IDisposable
    {
        private readonly List<Controllable> controllables = new List<Controllable>();

        private readonly List<uint> ignoredItems = new List<uint>();

        private readonly Manager manager;

        private readonly SnatcherMenu menu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        public Snatcher(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.SnatcherMenu;

            controllables.Add(new MyHero(manager.MyHero));

            if (this.menu.UseOtherUnits)
            {
                AddOtherUnits();

                manager.OnUnitRemove += OnUnitRemove;
                manager.OnUnitAdd += OnUnitAdd;
            }

            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;
            this.menu.OnUseOtherUnitsChange += OnUseOtherUnitsChange;
        }

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
            menu.OnUseOtherUnitsChange -= OnUseOtherUnitsChange;
            manager.OnUnitAdd -= OnUnitAdd;
            manager.OnUnitRemove -= OnUnitRemove;

            controllables.Clear();
            ignoredItems.Clear();
        }

        private void AddOtherUnits()
        {
            var controllableUnits = ObjectManager.GetEntities<Unit>()
                .Where(x => x.IsValid && x.IsControllable && !x.IsIllusion)
                .ToList();

            var spiritBear = controllableUnits.FirstOrDefault(x => x.ClassId == ClassId.CDOTA_Unit_SpiritBear);
            if (spiritBear != null)
            {
                controllables.Add(new SpiritBear(spiritBear));
            }

            foreach (var meepo in controllableUnits.Where(
                x => x.ClassId == ClassId.CDOTA_Unit_Hero_Meepo && x.Handle != manager.MyHandle))
            {
                controllables.Add(new MeepoClone(meepo));
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.IsPlayerInput)
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

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping(this) || Game.IsPaused)
            {
                return;
            }

            sleeper.Sleep(menu.Delay, this);

            var validControllables = controllables.Where(x => x.IsValid()).ToList();

            if (menu.ToggleKey && menu.EnabledToggleItems.Contains(0)
                || menu.HoldKey && menu.EnabledHoldItems.Contains(0))
            {
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

            var items = ObjectManager.GetEntitiesParallel<PhysicalItem>()
                .Where(
                    x => x.IsVisible && !ignoredItems.Contains(x.Item.Handle) && !sleeper.Sleeping(x.Handle)
                         && (menu.ToggleKey && menu.EnabledToggleItems.Contains(x.Item.Id)
                             || menu.HoldKey && menu.EnabledHoldItems.Contains(x.Item.Id)));

            foreach (var item in items)
            {
                foreach (var controllable in validControllables)
                {
                    if (controllable.CanPick(item))
                    {
                        controllable.Pick(item);
                        sleeper.Sleep(500, item.Handle);
                        break;
                    }
                }
            }
        }

        private void OnUseOtherUnitsChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                AddOtherUnits();
                manager.OnUnitAdd += OnUnitAdd;
                manager.OnUnitRemove += OnUnitRemove;
            }
            else
            {
                controllables.RemoveAll(x => x.Handle != manager.MyHandle);
                manager.OnUnitAdd -= OnUnitAdd;
                manager.OnUnitRemove -= OnUnitRemove;
            }
        }
    }
}
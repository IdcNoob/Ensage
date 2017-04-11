namespace ItemManager.Core.Modules.Snatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Controllables;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus.Modules.Snatcher;

    internal class Snatcher
    {
        private readonly List<Controllable> controllables = new List<Controllable>();

        private readonly List<uint> ignoredItems = new List<uint>();

        private readonly SnatcherMenu menu;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private ItemManager items;

        public Snatcher(Hero myHero, ItemManager itemManager, SnatcherMenu snatcherMenu)
        {
            items = itemManager;
            menu = snatcherMenu;

            controllables.Add(new MyHero(myHero));

            if (menu.UseOtherUnits)
            {
                AddOtherUnits();
                ObjectManager.OnAddEntity += OnAddEntity;
                ObjectManager.OnRemoveEntity += OnRemoveEntity;
            }

            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;
            menu.OnUseOtherUnitsChange += MenuOnUseOtherUnitsChange;
        }

        public void OnClose()
        {
            ObjectManager.OnAddEntity -= OnAddEntity;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
            menu.OnUseOtherUnitsChange -= MenuOnUseOtherUnitsChange;

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
                x => x.ClassId == ClassId.CDOTA_Unit_Hero_Meepo && x.Handle != ObjectManager.LocalHero.Handle))
            {
                controllables.Add(new MeepoClone(meepo));
            }
        }

        private void MenuOnUseOtherUnitsChange(object sender, EventArgs eventArgs)
        {
            if (menu.UseOtherUnits)
            {
                AddOtherUnits();
                ObjectManager.OnAddEntity += OnAddEntity;
                ObjectManager.OnRemoveEntity += OnRemoveEntity;
            }
            else
            {
                controllables.RemoveAll(x => x.Handle != ObjectManager.LocalHero.Handle);
                ObjectManager.OnAddEntity -= OnAddEntity;
                ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            }
        }

        private void OnAddEntity(EntityEventArgs args)
        {
            var unit = args.Entity as Unit;
            if (unit == null || unit.IsIllusion || !unit.IsControllable
                || controllables.Any(x => x.Handle == unit.Handle))
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

        private void OnRemoveEntity(EntityEventArgs args)
        {
            var controllable = controllables.FirstOrDefault(x => x.Handle == args.Entity.Handle);
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
    }
}
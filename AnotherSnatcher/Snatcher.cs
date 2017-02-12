namespace AnotherSnatcher
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Controllables;

    using Ensage;
    using Ensage.Common.Objects.UtilityObjects;

    internal class Snatcher
    {
        #region Fields

        private readonly List<Controllable> controllables = new List<Controllable>();

        private readonly List<uint> ignoredItems = new List<uint>();

        private MenuManager menu;

        private MultiSleeper sleeper;

        #endregion

        #region Public Methods and Operators

        public void OnClose()
        {
            ObjectManager.OnAddEntity -= OnAddEntity;
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
            menu.OnUseOtherUnitsChange -= MenuOnUseOtherUnitsChange;

            controllables.Clear();
            ignoredItems.Clear();
            menu.OnClose();
        }

        public void OnLoad()
        {
            menu = new MenuManager();
            sleeper = new MultiSleeper();
            controllables.Add(new MyHero(ObjectManager.LocalHero));

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

        #endregion

        #region Methods

        private void AddOtherUnits()
        {
            var contrallableUnits =
                ObjectManager.GetEntities<Unit>().Where(x => x.IsValid && x.IsControllable && !x.IsIllusion).ToList();

            var spiritBear = contrallableUnits.FirstOrDefault(x => x.ClassID == ClassID.CDOTA_Unit_SpiritBear);
            if (spiritBear != null)
            {
                controllables.Add(new SpiritBear(spiritBear));
            }

            foreach (var meepo in
                contrallableUnits.Where(
                    x => x.ClassID == ClassID.CDOTA_Unit_Hero_Meepo && x.Handle != ObjectManager.LocalHero.Handle))
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
                || controllables.Exists(x => x.Handle == unit.Handle))
            {
                return;
            }

            switch (unit.ClassID)
            {
                case ClassID.CDOTA_Unit_SpiritBear:
                    controllables.Add(new SpiritBear(unit));
                    break;
                case ClassID.CDOTA_Unit_Hero_Meepo:
                    controllables.Add(new MeepoClone(unit));
                    break;
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (args.Order == Order.DropItem)
            {
                ignoredItems.Add(args.Ability.Handle);
            }
            else if (args.Order == Order.PickItem)
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
                var runes =
                    ObjectManager.GetEntitiesParallel<Rune>().Where(x => !sleeper.Sleeping(x.Handle) && x.IsVisible);

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

            var items =
                ObjectManager.GetEntitiesParallel<PhysicalItem>()
                    .Where(
                        x =>
                            x.IsVisible && !ignoredItems.Contains(x.Item.Handle) && !sleeper.Sleeping(x.Handle)
                            && (menu.ToggleKey && menu.EnabledToggleItems.Contains(x.Item.ID)
                                || menu.HoldKey && menu.EnabledHoldItems.Contains(x.Item.ID)));

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

        #endregion
    }
}
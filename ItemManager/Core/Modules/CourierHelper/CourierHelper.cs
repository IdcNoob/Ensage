namespace ItemManager.Core.Modules.CourierHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus;
    using Menus.Modules.CourierHelper;

    using Utils;

    [Module]
    internal class CourierHelper : IDisposable
    {
        private readonly List<Courier> couriers = new List<Courier>();

        private readonly Manager manager;

        private readonly CourierHelperMenu menu;

        private readonly Sleeper sleeper;

        private bool bottleAbuseActive;

        private bool courierFollowing;

        public CourierHelper(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.CourierHelperMenu;
            sleeper = new Sleeper();

            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            this.menu.OnBottleAbuse += OnBottleAbuse;

            manager.OnUnitAdd += OnUnitAdd;
        }

        public void Dispose()
        {
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            Game.OnUpdate -= OnUpdate;
            menu.OnBottleAbuse -= OnBottleAbuse;

            couriers.Clear();
        }

        private void DisableAbuse()
        {
            bottleAbuseActive = false;
            courierFollowing = false;
            Game.OnUpdate -= OnUpdate;
        }

        private void OnBottleAbuse(object sender, EventArgs eventArgs)
        {
            if (bottleAbuseActive)
            {
                courierFollowing = false;
                return;
            }

            bottleAbuseActive = true;
            Game.OnUpdate += OnUpdate;
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.OldValue == args.NewValue || args.PropertyName != "m_nCourierState")
            {
                return;
            }

            var courier = sender as Courier;
            if (courier == null || !courier.IsAlive)
            {
                return;
            }

            if (bottleAbuseActive)
            {
                DisableAbuse();
            }

            if (!menu.AutoControl)
            {
                return;
            }

            var newState = (CourierState)args.NewValue;
            var oldState = (CourierState)args.OldValue;

            if (oldState != CourierState.Deliver || newState != CourierState.BackToBase)
            {
                return;
            }

            var purchaser = courier.Inventory.Items.Select(x => x.Purchaser?.Hero)
                .OrderByDescending(x => x?.Handle == manager.MyHero.Handle)
                .FirstOrDefault(x => x?.Team == manager.MyHero.Team);

            if (purchaser == null)
            {
                return;
            }

            if (purchaser.Handle == manager.MyHero.Handle)
            {
                courier.DeliverItems();
            }
            else
            {
                courier.Resend(purchaser);
            }
        }

        private void OnUnitAdd(object sender, UnitEventArgs unitEventArgs)
        {
            var courier = unitEventArgs.Unit as Courier;
            if (courier != null && courier.Team == manager.MyHero.Team)
            {
                couriers.Add(courier);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            var courier = couriers.FirstOrDefault(x => x.IsValid && x.IsAlive);
            if (courier == null || !manager.MyHero.IsAlive)
            {
                DisableAbuse();
                return;
            }

            var myBottle = manager.MyHero.GetMyItems(ItemStoredPlace.Inventory | ItemStoredPlace.Backpack)
                .FirstOrDefault(x => x.Id == AbilityId.item_bottle);

            var courierBottle = courier.Inventory.Items.FirstOrDefault(
                x => x.Id == AbilityId.item_bottle && x.Purchaser?.Hero?.Handle == manager.MyHero.Handle);

            if (myBottle == null && courierBottle == null)
            {
                DisableAbuse();
                return;
            }

            if (myBottle != null)
            {
                if (!courierFollowing)
                {
                    courier.Follow(manager.MyHero.Hero);
                    courierFollowing = true;
                }

                if (courier.Distance2D(manager.MyHero.Hero) > 250 || myBottle.CurrentCharges > 0)
                {
                    sleeper.Sleep(200);
                    return;
                }

                manager.MyHero.Hero.GiveItem(myBottle, courier);
                sleeper.Sleep(300);
                return;
            }

            courier.Burst();

            if (manager.MyHero.GetMyItems(ItemStoredPlace.Stash).Any())
            {
                courier.TakeAndDeliverItems();
            }
            else
            {
                if (courierBottle.CurrentCharges <= 0)
                {
                    courier.MoveToBase();
                }
                courier.DeliverItems(true);
            }

            DisableAbuse();
        }
    }
}
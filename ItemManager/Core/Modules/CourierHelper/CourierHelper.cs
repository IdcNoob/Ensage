namespace ItemManager.Core.Modules.CourierHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;

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

        private bool bottleAbuseActive;

        public CourierHelper(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.CourierHelperMenu;

            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            this.menu.OnBottleAbuse += OnBottleAbuse;

            manager.OnUnitAdd += OnUnitAdd;
        }

        public void Dispose()
        {
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            menu.OnBottleAbuse -= OnBottleAbuse;

            couriers.Clear();
        }

        private async void OnBottleAbuse(object sender, EventArgs eventArgs)
        {
            if (bottleAbuseActive)
            {
                return;
            }

            var courier = couriers.FirstOrDefault(x => x.IsValid && x.IsAlive);
            if (courier == null)
            {
                return;
            }

            var myBottle = manager.GetMyItems(ItemUtils.StoredPlace.Inventory | ItemUtils.StoredPlace.Backpack)
                .FirstOrDefault(x => x.Id == AbilityId.item_bottle);

            if (myBottle == null)
            {
                return;
            }

            bottleAbuseActive = true;
            courier.Follow(manager.MyHero);

            while (bottleAbuseActive)
            {
                if (!courier.IsAlive || !manager.MyHero.IsAlive)
                {
                    bottleAbuseActive = false;
                    return;
                }

                if (courier.Inventory.Items.All(
                    x => x.Id != AbilityId.item_bottle || x.Purchaser?.Hero.Handle != manager.MyHandle))
                {
                    if (courier.Distance2D(manager.MyHero) > 250 || myBottle.CurrentCharges > 0)
                    {
                        await Task.Delay(200);
                        continue;
                    }

                    manager.MyHero.GiveItem(myBottle, courier);
                    await Task.Delay(300);
                }
                else
                {
                    courier.Burst();

                    if (manager.GetMyItems(ItemUtils.StoredPlace.Stash).Any())
                    {
                        courier.TakeAndDeliverItems();
                    }
                    else
                    {
                        courier.MoveToBase();
                        courier.DeliverItems(true);
                    }

                    bottleAbuseActive = false;
                }
            }
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
                bottleAbuseActive = false;
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
                .OrderByDescending(x => x?.Handle == manager.MyHandle)
                .FirstOrDefault(x => x?.Team == manager.MyTeam);

            if (purchaser == null)
            {
                return;
            }

            if (purchaser.Handle == manager.MyHandle)
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
            if (courier != null && courier.Team == manager.MyTeam)
            {
                couriers.Add(courier);
            }
        }
    }
}
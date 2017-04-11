namespace ItemManager.Core.Modules.CourierHelper
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Extensions;

    using Menus.Modules.CourierHelper;

    using Utils;

    internal class CourierHelper
    {
        private readonly Hero hero;

        private readonly ItemManager items;

        private readonly CourierHelperMenu menu;

        private bool bottleAbuseActive;

        public CourierHelper(Hero myHero, ItemManager itemManager, CourierHelperMenu courierHelperMenu)
        {
            hero = myHero;
            items = itemManager;
            menu = courierHelperMenu;

            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            menu.OnBottleAbuse += OnBottleAbuse;
        }

        public void OnClose()
        {
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            menu.OnBottleAbuse -= OnBottleAbuse;
        }

        private async void OnBottleAbuse(object sender, EventArgs eventArgs)
        {
            if (bottleAbuseActive)
            {
                return;
            }

            var courier = ObjectManager.GetEntities<Courier>()
                .FirstOrDefault(x => x.IsValid && x.Team == hero.Team && x.IsAlive);

            if (courier == null)
            {
                return;
            }

            var myBottle = items.GetMyItems(ItemUtils.StoredPlace.Inventory | ItemUtils.StoredPlace.Backpack)
                .FirstOrDefault(x => x.Id == AbilityId.item_bottle);

            if (myBottle == null)
            {
                return;
            }

            bottleAbuseActive = true;
            courier.Follow(hero);

            while (bottleAbuseActive)
            {
                if (!courier.IsAlive || !hero.IsAlive)
                {
                    bottleAbuseActive = false;
                    return;
                }

                if (courier.Inventory.Items.All(
                    x => x.Id != AbilityId.item_bottle || x.Purchaser?.Hero.Handle != hero.Handle))
                {
                    if (courier.Distance2D(hero) > 250 || myBottle.CurrentCharges > 0)
                    {
                        await Task.Delay(200);
                        continue;
                    }

                    hero.GiveItem(myBottle, courier);
                    await Task.Delay(300);
                }
                else
                {
                    courier.Burst();

                    if (items.GetMyItems(ItemUtils.StoredPlace.Stash).Any())
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

            var newState = (CourierState)args.NewValue;
            var oldState = (CourierState)args.OldValue;

            if (bottleAbuseActive)
            {
                bottleAbuseActive = false;
            }

            if (oldState != CourierState.Deliver || newState != CourierState.BackToBase)
            {
                return;
            }

            var purchaser = courier.Inventory.Items.Select(x => x.Purchaser?.Hero)
                .OrderByDescending(x => x?.Handle == hero.Handle)
                .FirstOrDefault(x => x?.Team == courier.Team);

            if (purchaser == null)
            {
                return;
            }

            if (purchaser.Handle == hero.Handle)
            {
                courier.DeliverItems();
            }
            else
            {
                courier.Resend(purchaser);
            }
        }
    }
}
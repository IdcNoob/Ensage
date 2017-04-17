namespace ItemManager.Core.Modules.AbilityHelper
{
    using System;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus;
    using Menus.Modules.AbilityHelper;

    using Utils;

    [Module]
    internal class TranquilDrop : IDisposable
    {
        private readonly Manager manager;

        private readonly TranquilMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private bool subscribed;

        public TranquilDrop(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AbilityHelperMenu.TranquilMenu;

            this.menu.OnTranquilDrop += OnTranquilDrop;
        }

        public void Dispose()
        {
            menu.OnTranquilDrop -= OnTranquilDrop;
            Game.OnUpdate -= OnUpdate;
        }

        private void OnTranquilDrop(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                var tranquils = manager.GetMyItems(ItemUtils.StoredPlace.Inventory)
                    .FirstOrDefault(x => x.Id == AbilityId.item_tranquil_boots);

                if (tranquils == null)
                {
                    return;
                }

                manager.MyHero.Stop();
                manager.DropItem(tranquils, ItemUtils.StoredPlace.Inventory);

                if (subscribed)
                {
                    return;
                }

                Game.OnUpdate += OnUpdate;
                subscribed = true;
            }
            else
            {
                manager.PickUpItems();

                if (!subscribed)
                {
                    return;
                }

                Game.OnUpdate -= OnUpdate;
                subscribed = false;
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Game.IsPaused || sleeper.Sleeping)
            {
                return;
            }

            if (ObjectManager.GetEntitiesParallel<Hero>()
                .Any(
                    x => x.IsValid && x.IsValid && x.IsAlive && x.Team != manager.MyTeam
                         && x.Distance2D(manager.MyHero) < 800))
            {
                manager.PickUpItems();
                sleeper.Sleep(500);
            }
        }
    }
}
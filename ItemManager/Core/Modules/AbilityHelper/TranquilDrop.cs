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
                var tranquils = manager.MyHero.GetMyItems(ItemStoredPlace.Inventory)
                    .FirstOrDefault(x => x.Id == AbilityId.item_tranquil_boots);

                if (tranquils == null)
                {
                    return;
                }

                manager.MyHero.Hero.Stop();
                manager.MyHero.DropItem(tranquils, ItemStoredPlace.Inventory);

                if (subscribed)
                {
                    return;
                }

                Game.OnUpdate += OnUpdate;
                subscribed = true;
            }
            else
            {
                manager.MyHero.PickUpItems();

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
                    x => x.IsValid && x.IsValid && x.IsAlive && x.Team != manager.MyHero.Team
                         && x.Distance2D(manager.MyHero.Position) < 800))
            {
                manager.MyHero.PickUpItems();
                sleeper.Sleep(500);
            }
        }
    }
}
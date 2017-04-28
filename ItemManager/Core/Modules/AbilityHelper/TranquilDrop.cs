namespace ItemManager.Core.Modules.AbilityHelper
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus;
    using Menus.Modules.AbilityHelper;

    using Utils;

    // [AbilityBasedModule(AbilityId.item_tranquil_boots)]

    internal class TranquilDrop //: IAbilityBasedModule
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

        public AbilityId AbilityId { get; } = AbilityId.item_tranquil_boots;

        public void Dispose()
        {
            menu.OnTranquilDrop -= OnTranquilDrop;
            Game.OnUpdate -= OnUpdate;
        }

        public void Refresh()
        {
        }

        private void OnTranquilDrop(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                var tranquils = manager.MyHero.GetItems(ItemStoredPlace.Inventory)
                    .FirstOrDefault(x => x.Id == AbilityId);

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
            if (sleeper.Sleeping || Game.IsPaused)
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
namespace ItemManager.Core.Modules.AbilityHelper
{
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AbilityHelper;

    using Utils;

    [AbilityBasedModule(AbilityId.item_tranquil_boots)]
    internal class TranquilDrop : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly TranquilMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private readonly IUpdateHandler updateHandler;

        public TranquilDrop(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AbilityHelperMenu.TranquilMenu;

            AbilityId = abilityId;

            updateHandler = UpdateManager.Subscribe(OnUpdate, 100, false);
            this.menu.OnTranquilDrop += OnTranquilDrop;
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            menu.OnTranquilDrop -= OnTranquilDrop;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
        }

        private void OnTranquilDrop(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                var tranquils = manager.MyHero.GetItems(ItemStoredPlace.Inventory).FirstOrDefault(x => x.Id == AbilityId);

                if (tranquils == null)
                {
                    return;
                }

                manager.MyHero.Hero.Stop();
                manager.MyHero.DropItem(tranquils, ItemStoredPlace.Inventory);
                updateHandler.IsEnabled = true;
            }
            else
            {
                manager.MyHero.PickUpItems();
                updateHandler.IsEnabled = false;
            }
        }

        private void OnUpdate()
        {
            if (sleeper.Sleeping || Game.IsPaused)
            {
                return;
            }

            if (EntityManager<Hero>.Entities.Any(
                x => x.IsValid && x.IsVisible && x.IsAlive && x.Team != manager.MyHero.Team && x.Distance2D(manager.MyHero.Position) < 800))
            {
                manager.MyHero.PickUpItems();
                sleeper.Sleep(500);
            }
        }
    }
}
namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [Module]
    internal class AutoIronTalon : IDisposable
    {
        private readonly Manager manager;

        private readonly IronTalonMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private IronTalon ironTalon;

        private bool subscribed;

        public AutoIronTalon(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.IronTalonMenu;

            manager.OnItemAdd += OnItemAdd;
            manager.OnItemRemove += OnItemRemove;
        }

        public void Dispose()
        {
            manager.OnItemAdd -= OnItemAdd;
            manager.OnItemRemove -= OnItemRemove;
            Game.OnUpdate -= OnUpdate;
        }

        private void OnItemAdd(object sender, ItemEventArgs itemEventArgs)
        {
            if (!itemEventArgs.IsMine)
            {
                return;
            }

            if (itemEventArgs.Item.Id == AbilityId.item_iron_talon)
            {
                ironTalon =
                    manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_iron_talon) as IronTalon;

                if (ironTalon != null && !subscribed)
                {
                    subscribed = true;
                    Game.OnUpdate += OnUpdate;
                }
            }
        }

        private void OnItemRemove(object sender, ItemEventArgs itemEventArgs)
        {
            if (!itemEventArgs.IsMine)
            {
                return;
            }

            if (itemEventArgs.Item.Id == AbilityId.item_iron_talon)
            {
                ironTalon =
                    manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_iron_talon) as IronTalon;

                if (ironTalon == null && subscribed)
                {
                    subscribed = false;
                    Game.OnUpdate -= OnUpdate;
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(500);

            if (!menu.IsEnabled || !manager.MyHero.CanUseItems() || !ironTalon.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            var creep = ObjectManager.GetEntitiesParallel<Creep>()
                .Where(
                    x => x.IsValid && x.IsAlive && x.IsSpawned && x.IsVisible
                         && x.Distance2D(manager.MyHero.Position) <= ironTalon.GetCastRange()
                         && x.Team != manager.MyHero.Team && !x.IsAncient
                         && x.Health * ironTalon.Damage >= menu.DamageThreshold)
                .OrderByDescending(x => x.Health)
                .FirstOrDefault();

            if (creep != null)
            {
                ironTalon.Use(creep);
            }
        }
    }
}
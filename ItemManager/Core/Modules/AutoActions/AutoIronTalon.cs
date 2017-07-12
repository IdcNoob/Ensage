namespace ItemManager.Core.Modules.AutoActions
{
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [AbilityBasedModule(AbilityId.item_iron_talon)]
    internal class AutoIronTalon : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly IronTalonMenu menu;

        private readonly IUpdateHandler updateHandler;

        private IronTalon ironTalon;

        public AutoIronTalon(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.IronTalonMenu;

            AbilityId = abilityId;
            Refresh();

            updateHandler = UpdateManager.Subscribe(OnUpdate, 500, this.menu.IsEnabled);
            this.menu.OnEnabledChange += MenuOnEnabledChange;
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
            ironTalon = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as IronTalon;
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            updateHandler.IsEnabled = boolEventArgs.Enabled;
        }

        private void OnUpdate()
        {
            if (Game.IsPaused || !manager.MyHero.CanUseItems() || !ironTalon.CanBeCasted())
            {
                return;
            }

            var creep = EntityManager<Creep>.Entities.Where(
                    x => x.IsValid && x.IsVisible && x.IsAlive && x.IsSpawned
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
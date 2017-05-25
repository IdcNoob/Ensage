namespace ItemManager.Core.Modules.AutoActions
{
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [AbilityBasedModule(AbilityId.item_iron_talon)]
    internal class AutoIronTalon : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly IronTalonMenu menu;

        private IronTalon ironTalon;

        public AutoIronTalon(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.IronTalonMenu;

            AbilityId = abilityId;
            Refresh();

            UpdateManager.Subscribe(OnUpdate, 500);
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
            ironTalon = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as IronTalon;
        }

        private void OnUpdate()
        {
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
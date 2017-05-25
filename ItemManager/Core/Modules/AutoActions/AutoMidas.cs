namespace ItemManager.Core.Modules.AutoActions
{
    using System.Linq;

    using Abilities.Base;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    using Utils;

    [AbilityBasedModule(AbilityId.item_hand_of_midas)]
    internal class AutoMidas : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly MidasMenu menu;

        private UsableAbility handOfMidas;

        public AutoMidas(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.MidasMenu;

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
            handOfMidas = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId);
        }

        private void OnUpdate()
        {
            if (!menu.IsEnabled || !manager.MyHero.CanUseItems() || !handOfMidas.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            var creeps = ObjectManager.GetEntitiesParallel<Creep>()
                .Where(
                    x => x.IsValid && x.IsAlive && x.IsSpawned && x.IsVisible
                         && x.Distance2D(manager.MyHero.Position) <= handOfMidas.GetCastRange()
                         && x.Team != manager.MyHero.Team && !x.IsAncient
                         && x.HealthPercentage() >= menu.HealthThresholdPct
                         && (x.GetGrantedExperience() >= menu.ExperienceThreshold || manager.MyHero.Level >= 25));

            var creep = manager.MyHero.Level >= 25
                            ? creeps.OrderBy(x => x.GetGrantedExperience()).FirstOrDefault()
                            : creeps.OrderByDescending(x => x.Health).FirstOrDefault();

            if (creep != null)
            {
                handOfMidas.Use(creep);
            }
        }
    }
}
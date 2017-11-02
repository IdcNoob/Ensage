namespace ItemManager.Core.Modules.AutoActions
{
    using System.Linq;

    using Abilities.Base;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    using Utils;

    [AbilityBasedModule(AbilityId.item_hand_of_midas)]
    internal class AutoMidas : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly MidasMenu menu;

        private readonly IUpdateHandler updateHandler;

        private UsableAbility handOfMidas;

        public AutoMidas(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.MidasMenu;

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
            handOfMidas = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId);
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            updateHandler.IsEnabled = boolEventArgs.Enabled;
        }

        private void OnUpdate()
        {
            if (!manager.MyHero.CanUseItems() || !handOfMidas.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            if (manager.MyHero.HasModifier(ModifierUtils.SpiritBreakerCharge))
            {
                return;
            }

            var creeps = EntityManager<Creep>.Entities.Where(
                x => x.IsValid && x.IsVisible && x.IsAlive && x.IsSpawned && !x.IsAncient
                     && x.Distance2D(manager.MyHero.Position) <= handOfMidas.GetCastRange() && x.Team != manager.MyHero.Team
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
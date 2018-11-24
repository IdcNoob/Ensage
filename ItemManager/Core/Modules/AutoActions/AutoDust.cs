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

    [AbilityBasedModule(AbilityId.item_dust)]
    internal class AutoDust : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly DustMenu menu;

        private readonly IUpdateHandler updateHandler;

        private UsableAbility dust;

        public AutoDust(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.DustMenu;

            AbilityId = abilityId;
            Refresh();

            updateHandler = UpdateManager.Subscribe(OnUpdate, 100, this.menu.IsEnabled);
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
            dust = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId);
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            updateHandler.IsEnabled = boolEventArgs.Enabled;
        }

        private void OnUpdate()
        {
            if (!manager.MyHero.CanUseItems() || !dust.CanBeCasted() || Game.IsPaused)
            {
                return;
            }

            var invisibleHero = EntityManager<Hero>.Entities.FirstOrDefault(
                x => x.IsValid && x.IsVisible && x.IsAlive && !x.IsIllusion && x.Team == manager.MyHero.EnemyTeam && x.InvisiblityLevel > 0
                     && x.Distance2D(manager.MyHero.Position) <= dust.GetCastRange() && !x.HasModifier(ModifierUtils.DustOfAppearance));

            if (invisibleHero == null)
            {
                return;
            }

            var allyWithGem = EntityManager<Hero>.Entities.Any(
                x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team && x.Distance2D(invisibleHero) < 800
                     && x.Inventory.Items.Any(z => z.Id == AbilityId.item_gem));

            if (allyWithGem)
            {
                return;
            }

            var sentryOrTower = EntityManager<Unit>.Entities.Any(
                x => x.IsValid && x.IsAlive && x.Team == manager.MyHero.Team
                     && (x.NetworkName == "CDOTA_BaseNPC_Tower" || x.NetworkName == "CDOTA_NPC_Observer_Ward_TrueSight")
                     && x.Distance2D(invisibleHero) < 800);

            if (sentryOrTower)
            {
                return;
            }

            dust.Use();
        }
    }
}
namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
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
    using Menus.Modules.AutoActions.HpMpRestore;

    [AbilityBasedModule(AbilityId.item_arcane_boots)]
    internal class AutoArcaneBoots : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly AutoArcaneBootsMenu menu;

        private readonly IUpdateHandler updateHandler;

        private ArcaneBoots arcaneBoots;

        private Unit fountain;

        private bool notified;

        public AutoArcaneBoots(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoArcaneBootsMenu;

            AbilityId = abilityId;
            Refresh();

            updateHandler = UpdateManager.Subscribe(OnUpdate, 500, this.menu.IsEnabled);
            if (this.menu.IsEnabled)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
            }
            this.menu.OnEnabledChange += MenuOnEnabledChange;
        }

        public AbilityId AbilityId { get; }

        private Unit Fountain
        {
            get
            {
                return fountain ?? (fountain = EntityManager<Building>.Entities.FirstOrDefault(
                                        x => x.IsValid && x.Name == "dota_fountain" && x.Team == manager.MyHero.Team));
            }
        }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            UpdateManager.Unsubscribe(OnUpdate);
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            arcaneBoots = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as ArcaneBoots;
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
                updateHandler.IsEnabled = true;
            }
            else
            {
                Player.OnExecuteOrder -= OnExecuteOrder;
                updateHandler.IsEnabled = false;
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero.Hero) || !args.Process || args.OrderId != OrderId.Ability)
            {
                return;
            }

            if (args.Ability?.Id == AbilityId)
            {
                notified = false;
            }
        }

        private void OnUpdate()
        {
            if (Game.IsPaused || !manager.MyHero.CanUseItems() || !arcaneBoots.CanBeCasted()
                || manager.MyHero.MissingMana < arcaneBoots.ManaRestore
                || Fountain != null && manager.MyHero.Distance2D(Fountain) < menu.FountainRange)
            {
                return;
            }

            if (EntityManager<Hero>.Entities.Any(
                x => x.IsValid && x.Handle != manager.MyHero.Handle && x.IsAlive && !x.IsIllusion && x.Team == manager.MyHero.Team
                     && x.Distance2D(manager.MyHero.Position) <= menu.AllySearchRange
                     && x.Distance2D(manager.MyHero.Position) > arcaneBoots.GetCastRange()
                     && x.MaximumMana - x.Mana > arcaneBoots.ManaRestore))
            {
                if (!notified && menu.NotifyAllies)
                {
                    Network.ItemAlert(manager.MyHero.Position, AbilityId);
                    UpdateManager.BeginInvoke(() => { Network.ItemAlert(manager.MyHero.Position, AbilityId); }, 200);
                    notified = true;
                }

                return;
            }

            arcaneBoots.Use();
        }
    }
}
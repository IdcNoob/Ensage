namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Linq;

    using Abilities.Base;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [AbilityBasedModule(AbilityId.item_phase_boots)]
    internal class AutoPhaseBoots : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly PhaseBootsMenu menu;

        private UsableAbility phaseBoots;

        public AutoPhaseBoots(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.PhaseBootsMenu;

            AbilityId = abilityId;
            Refresh();

            if (this.menu.IsEnabled)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
            }
            this.menu.OnEnabledChange += MenuOnEnabledChange;
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            phaseBoots = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_phase_boots);
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
            }
            else
            {
                Player.OnExecuteOrder -= OnExecuteOrder;
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero.Hero) || args.IsQueued || !args.Process)
            {
                return;
            }

            if (!phaseBoots.CanBeCasted() || !manager.MyHero.CanUseItems() || this.manager.MyHero.Hero.MovementSpeed <= 0)
            {
                return;
            }

            switch (args.OrderId)
            {
                case OrderId.AttackLocation:
                case OrderId.AttackTarget:
                {
                    var location = args.Target?.Position ?? args.TargetPosition;
                    if (Math.Max(manager.MyHero.Distance2D(location) - manager.MyHero.Hero.GetAttackRange(), 0) >= menu.Distance)
                    {
                        phaseBoots.Use();
                    }

                    break;
                }
                case OrderId.MoveTarget:
                case OrderId.MoveLocation:
                {
                    var location = args.Target?.Position ?? args.TargetPosition;
                    if (manager.MyHero.Distance2D(location) >= menu.Distance)
                    {
                        phaseBoots.Use();
                    }

                    break;
                }
            }
        }
    }
}
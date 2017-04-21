namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [AbilityBasedModule(AbilityId.item_phase_boots)]
    internal class AutoPhaseBoots : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly PhaseBootsMenu menu;

        private PhaseBoots phaseBoots;

        public AutoPhaseBoots(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.PhaseBootsMenu;

            Refresh();

            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.item_phase_boots
        };

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            phaseBoots =
                manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_phase_boots) as PhaseBoots;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.IsEnabled || !args.Entities.Contains(manager.MyHero.Hero) || args.IsQueued || !args.Process)
            {
                return;
            }

            if (!phaseBoots.CanBeCasted() || !manager.MyHero.CanUseItems())
            {
                return;
            }

            switch (args.OrderId)
            {
                case OrderId.AttackLocation:
                case OrderId.AttackTarget:
                {
                    var location = args.Target?.Position ?? args.TargetPosition;
                    if (Math.Max(manager.MyHero.Distance2D(location) - manager.MyHero.Hero.GetAttackRange(), 0)
                        >= menu.Distance)
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
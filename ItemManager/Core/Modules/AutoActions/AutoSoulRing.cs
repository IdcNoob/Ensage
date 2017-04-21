namespace ItemManager.Core.Modules.AutoActions
{
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Objects;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [AbilityBasedModule(AbilityId.item_soul_ring)]
    internal class AutoSoulRing : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly SoulRingMenu menu;

        private readonly Order order;

        private SoulRing soulRing;

        public AutoSoulRing(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.SoulRingMenu;
            order = new Order();

            Refresh();

            foreach (var ability in manager.MyHero.Abilities.Where(x => x.GetManaCost(0) > 0))
            {
                this.menu.AddAbility(ability.StoredName(), true);
            }

            manager.OnAbilityAdd += OnAbilityAdd;
            manager.OnAbilityRemove += OnAbilityRemove;
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.item_soul_ring
        };

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
            manager.OnAbilityAdd -= OnAbilityAdd;
            manager.OnAbilityRemove -= OnAbilityRemove;
        }

        public void Refresh()
        {
            soulRing = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityIds.First()) as SoulRing;
        }

        private void OnAbilityAdd(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine)
            {
                return;
            }

            if (abilityEventArgs.Ability.GetManaCost(0) > 0)
            {
                menu.AddAbility(abilityEventArgs.Ability.StoredName(), true);
            }
        }

        private void OnAbilityRemove(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine)
            {
                return;
            }

            if (abilityEventArgs.Ability.GetManaCost(0) > 0)
            {
                menu.RemoveAbility(abilityEventArgs.Ability.StoredName());
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.IsEnabled)
            {
                return;
            }

            switch (args.OrderId)
            {
                case OrderId.AbilityTarget:
                case OrderId.AbilityLocation:
                case OrderId.Ability:
                case OrderId.ToggleAbility:
                {
                    if (!args.Process || args.IsQueued || !args.Entities.Contains(manager.MyHero.Hero))
                    {
                        return;
                    }

                    var ability = args.Ability;

                    if (!args.IsPlayerInput)
                    {
                        if (ability.Id == AbilityId.item_soul_ring)
                        {
                            soulRing.SetSleep(1000);
                            return;
                        }

                        if (!menu.UniversalUseEnabled)
                        {
                            return;
                        }
                    }

                    if (!soulRing.CanBeCasted() || manager.MyHero.HealthPercentage < menu.HpThreshold
                        || manager.MyHero.ManaPercentage > menu.MpThreshold || manager.MyHero.IsInvisible())
                    {
                        return;
                    }

                    if (ability.ManaCost <= menu.MpAbilityThreshold || !menu.IsAbilityEnabled(ability.StoredName()))
                    {
                        return;
                    }

                    soulRing.Use();
                    order.Recast(args);

                    break;
                }
            }
        }
    }
}
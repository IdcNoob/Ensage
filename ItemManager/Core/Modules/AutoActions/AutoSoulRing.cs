namespace ItemManager.Core.Modules.AutoActions
{
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Objects;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    //[AbilityBasedModule(AbilityId.item_soul_ring)]
    internal class AutoSoulRing : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly SoulRingMenu menu;

        private readonly Order order;

        private SoulRing soulRing;

        public AutoSoulRing(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.SoulRingMenu;
            order = new Order();

            AbilityId = abilityId;
            Refresh();

            foreach (var ability in manager.MyHero.Abilities.Where(x => x.GetManaCost(0) > 0))
            {
                this.menu.AddAbility(ability.StoredName(), true);
            }

            manager.OnAbilityAdd += OnAbilityAdd;
            manager.OnAbilityRemove += OnAbilityRemove;
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
            manager.OnAbilityAdd -= OnAbilityAdd;
            manager.OnAbilityRemove -= OnAbilityRemove;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            soulRing = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as SoulRing;
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
            switch (args.OrderId)
            {
                case OrderId.AbilityTarget:
                case OrderId.AbilityLocation:
                case OrderId.Ability:
                case OrderId.ToggleAbility:
                {
                    if (!args.IsPlayerInput && !menu.UniversalUseEnabled || !args.Process || args.IsQueued
                        || !args.Entities.Contains(manager.MyHero.Hero))
                    {
                        return;
                    }

                    if (!soulRing.CanBeCasted() || manager.MyHero.HealthPercentage < menu.HpThreshold
                        || manager.MyHero.ManaPercentage > menu.MpThreshold || manager.MyHero.IsInvisible()
                        && !manager.MyHero.CanUseAbilitiesInInvisibility() && !menu.UseWhenInvisible)
                    {
                        return;
                    }

                    var ability = args.Ability;

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
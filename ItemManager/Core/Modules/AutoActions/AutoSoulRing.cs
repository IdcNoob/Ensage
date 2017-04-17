namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;

    using EventArgs;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [Module]
    internal class AutoSoulRing : IDisposable
    {
        private readonly Manager manager;

        private readonly SoulRingMenu menu;

        private readonly Order order;

        private SoulRing soulRing;

        private bool subscribed;

        public AutoSoulRing(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.SoulRingMenu;
            order = new Order();

            manager.OnAbilityAdd += OnAbilityAdd;
            manager.OnAbilityRemove += OnAbilityRemove;
        }

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
            manager.OnAbilityAdd -= OnAbilityAdd;
            manager.OnAbilityRemove -= OnAbilityRemove;
        }

        private bool CheckHpThreshold()
        {
            return (float)manager.MyHero.Health / manager.MyHero.MaximumHealth * 100 >= menu.HpThreshold;
        }

        private bool CheckMpThreshold()
        {
            return manager.MyHero.Mana / manager.MyHero.MaximumMana * 100 <= menu.MpThreshold;
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

            if (abilityEventArgs.Ability.Id == AbilityId.item_soul_ring)
            {
                soulRing = manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_soul_ring) as SoulRing;

                if (soulRing != null && !subscribed)
                {
                    subscribed = true;
                    Player.OnExecuteOrder += OnExecuteOrder;
                }
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

            if (abilityEventArgs.Ability.Id == AbilityId.item_soul_ring)
            {
                soulRing = manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_soul_ring) as SoulRing;

                if (soulRing == null)
                {
                    subscribed = false;
                    Player.OnExecuteOrder -= OnExecuteOrder;
                }
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
                    if (!args.Process || args.IsQueued || !args.Entities.Contains(manager.MyHero))
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

                    if (!soulRing.CanBeCasted() || !CheckHpThreshold() || !CheckMpThreshold()
                        || manager.MyHero.IsInvisible())
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
namespace ItemManager.Core.Modules.ShrineHelper
{
    using System;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Menus;
    using Menus.Modules.ShrineHelper;

    using Utils;

    [Module]
    internal class ShrineHelper : IDisposable
    {
        private readonly Manager manager;

        private readonly ShrineHelperMenu menu;

        public ShrineHelper(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.ShrineHelperMenu;

            if (this.menu.BlockShrineUsage)
            {
                Player.OnExecuteOrder += OnExecuteOrder;
            }
            if (this.menu.AutoDisableItems)
            {
                Unit.OnModifierAdded += OnModifierAdded;
            }

            this.menu.OnBlockEnabledChange += MenuOnBlockEnabledChange;
            this.menu.OnDisableItemsChange += MenuOnDisableItemsChange;
        }

        public void Dispose()
        {
            menu.OnBlockEnabledChange -= MenuOnBlockEnabledChange;
            menu.OnDisableItemsChange -= MenuOnDisableItemsChange;
            Player.OnExecuteOrder -= OnExecuteOrder;
            Unit.OnModifierAdded -= OnModifierAdded;
        }

        private void MenuOnBlockEnabledChange(object sender, BoolEventArgs boolEventArgs)
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

        private void MenuOnDisableItemsChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                Unit.OnModifierAdded += OnModifierAdded;
            }
            else
            {
                Unit.OnModifierAdded -= OnModifierAdded;
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.IsPlayerInput || !args.Entities.Contains(manager.MyHero.Hero))
            {
                return;
            }

            if (args.OrderId == OrderId.MoveTarget && args.Target?.NetworkName == "CDOTA_BaseNPC_Healer"
                && manager.MyHero.HealthPercentage > menu.HpUseThreshold && manager.MyHero.ManaPercentage > menu.MpUseThreshold)
            {
                args.Process = false;
                manager.MyHero.Hero.Move(args.Target.Position);
            }
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (sender.Handle != manager.MyHero.Handle || args.Modifier.Name != ModifierUtils.ShrineRegeneration)
            {
                return;
            }

            if (manager.MyHero.HealthPercentage > menu.HpDisableThreshold && manager.MyHero.ManaPercentage > menu.MpDisableThreshold)
            {
                return;
            }

            if (EntityManager<Hero>.Entities.Any(
                x => x.IsValid && x.IsVisible && x.IsAlive && x.Team != manager.MyHero.Team
                     && x.Distance2D(manager.MyHero.Position) < 1000))
            {
                return;
            }

            if (manager.MyHero.ItemsCanBeDisabled())
            {
                manager.MyHero.DropItems(ItemStats.Any, true);
            }
        }
    }
}
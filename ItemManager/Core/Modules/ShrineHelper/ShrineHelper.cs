namespace ItemManager.Core.Modules.ShrineHelper
{
    using System;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;

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

            Player.OnExecuteOrder += OnExecuteOrder;
            Unit.OnModifierAdded += OnModifierAdded;
        }

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
            Unit.OnModifierAdded -= OnModifierAdded;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.BlockShrineUsage || !args.IsPlayerInput || !args.Entities.Contains(manager.MyHero.Hero))
            {
                return;
            }

            if (args.OrderId == OrderId.MoveTarget && args.Target?.ClassId == ClassId.CDOTA_BaseNPC_Healer
                && manager.MyHero.HealthPercentage > menu.HpThreshold
                && manager.MyHero.ManaPercentage > menu.MpThreshold)
            {
                args.Process = false;
                manager.MyHero.Hero.Move(args.Target.Position);
            }
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (!menu.AutoDisableItems || sender.Handle != manager.MyHero.Handle
                || args.Modifier.Name != ModifierUtils.ShrineRegeneration)
            {
                return;
            }

            if (manager.MyHero.HealthPercentage > 80 && manager.MyHero.ManaPercentage > 80)
            {
                return;
            }

            if (ObjectManager.GetEntitiesParallel<Hero>()
                .Any(
                    x => x.IsValid && x.IsVisible && x.IsAlive && x.Team != manager.MyHero.Team
                         && x.Distance2D(manager.MyHero.Position) < 1000))
            {
                return;
            }

            manager.MyHero.DropItems(ItemStats.Any, true);
        }
    }
}
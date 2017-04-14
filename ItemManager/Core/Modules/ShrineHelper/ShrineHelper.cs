namespace ItemManager.Core.Modules.ShrineHelper
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using Menus.Modules.ShrineHelper;

    using Utils;

    internal class ShrineHelper : IDisposable
    {
        private readonly Manager manager;

        private readonly ShrineHelperMenu menu;

        public ShrineHelper(Manager manager, ShrineHelperMenu shrineHelperMenu)
        {
            this.manager = manager;
            menu = shrineHelperMenu;

            Player.OnExecuteOrder += OnExecuteOrder;
            Unit.OnModifierAdded += OnModifierAdded;
        }

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.BlockShrineUsage || !args.IsPlayerInput || !args.Entities.Contains(manager.MyHero))
            {
                return;
            }

            if (args.OrderId == OrderId.MoveTarget && args.Target?.ClassId == ClassId.CDOTA_BaseNPC_Healer
                && manager.MyHealthPercentage > menu.HpThreshold && manager.MyManaPercentage > menu.MpThreshold)
            {
                args.Process = false;
                manager.MyHero.Move(args.Target.Position);
            }
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (!menu.AutoDisableItems || sender.Handle != manager.MyHero.Handle
                || args.Modifier.Name != ModifierUtils.ShrineRegeneration)
            {
                return;
            }

            if (manager.MyHealthPercentage > 80 && manager.MyManaPercentage > 80)
            {
                return;
            }

            if (ObjectManager.GetEntities<Hero>()
                .Any(
                    x => x.IsValid && x.IsVisible && x.IsAlive && x.Team != manager.MyTeam
                         && x.Distance2D(manager.MyHero) < 1000))
            {
                return;
            }

            manager.DropItems(ItemUtils.Stats.Any, true);
        }
    }
}
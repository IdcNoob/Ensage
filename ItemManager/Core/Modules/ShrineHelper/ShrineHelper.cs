namespace ItemManager.Core.Modules.ShrineHelper
{
    using System.Linq;

    using Ensage;

    using Menus.Modules.ShrineHelper;

    internal class ShrineHelper
    {
        private readonly Hero hero;

        private readonly ShrineHelperMenu menu;

        public ShrineHelper(Hero myHero, ShrineHelperMenu shrineHelperMenu)
        {
            hero = myHero;
            menu = shrineHelperMenu;

            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public void OnClose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.BlockShrineUsage || !args.IsPlayerInput || !args.Entities.Contains(hero))
            {
                return;
            }

            if (args.OrderId == OrderId.MoveTarget && args.Target?.ClassId == ClassId.CDOTA_BaseNPC_Healer
                && (float)hero.Health / hero.MaximumHealth * 100 > menu.HpThreshold
                && hero.Mana / hero.MaximumMana * 100 > menu.MpThreshold)
            {
                args.Process = false;
                hero.Move(args.Target.Position);
            }
        }
    }
}
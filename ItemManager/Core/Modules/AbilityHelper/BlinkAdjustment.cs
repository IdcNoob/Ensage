namespace ItemManager.Core.Modules.AbilityHelper
{
    using System;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Menus;
    using Menus.Modules.AbilityHelper;

    [Module]
    internal class BlinkAdjustment : IDisposable
    {
        private readonly Manager manager;

        private readonly BlinkMenu menu;

        public BlinkAdjustment(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AbilityHelperMenu.BlinkMenu;

            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.IsEnabled || !args.Entities.Contains(manager.MyHero.Hero) || args.IsQueued || !args.Process)
            {
                return;
            }

            if (args.OrderId == OrderId.AbilityLocation && args.Ability?.Id == AbilityId.item_blink)
            {
                var location = args.TargetPosition;
                var blink = args.Ability;
                var castRange = blink.GetCastRange();

                if (manager.MyHero.Distance2D(location) <= castRange)
                {
                    return;
                }

                args.Process = false;
                blink.UseAbility(manager.MyHero.Position.Extend(location, castRange - 10));
            }
        }
    }
}
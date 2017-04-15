namespace ItemManager.Core.Modules.AbilityHelper
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using Menus.Modules.AbilityHelper;

    internal class BlinkAdjustment : IDisposable
    {
        private readonly Manager manager;

        private readonly Blink menu;

        public BlinkAdjustment(Manager manager, Blink menu)
        {
            this.manager = manager;
            this.menu = menu;

            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.IsEnabled || !args.Entities.Contains(manager.MyHero) || args.IsQueued)
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
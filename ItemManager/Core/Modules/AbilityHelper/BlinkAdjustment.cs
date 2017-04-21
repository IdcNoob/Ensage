namespace ItemManager.Core.Modules.AbilityHelper
{
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions.SharpDX;

    using Interfaces;

    using Menus;
    using Menus.Modules.AbilityHelper;

    [AbilityBasedModule(AbilityId.item_blink)]
    internal class BlinkAdjustment : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly BlinkMenu menu;

        private BlinkDagger blinkDagger;

        public BlinkAdjustment(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AbilityHelperMenu.BlinkMenu;

            Refresh();

            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.item_blink
        };

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            blinkDagger = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityIds.First()) as BlinkDagger;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!menu.IsEnabled || !args.Entities.Contains(manager.MyHero.Hero) || args.IsQueued || !args.Process)
            {
                return;
            }

            if (args.OrderId == OrderId.AbilityLocation && args.Ability?.Id == AbilityIds.First())
            {
                var location = args.TargetPosition;
                var castRange = blinkDagger.GetCastRange();

                if (manager.MyHero.Distance2D(location) <= castRange)
                {
                    return;
                }

                args.Process = false;
                blinkDagger.Use(manager.MyHero.Position.Extend(location, castRange - 10));
            }
        }
    }
}
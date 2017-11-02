namespace ItemManager.Core.Modules.AbilityHelper
{
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions.SharpDX;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AbilityHelper;

    [AbilityBasedModule(AbilityId.item_blink)]
    internal class BlinkAdjustment : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly BlinkMenu menu;

        private BlinkDagger blinkDagger;

        public BlinkAdjustment(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AbilityHelperMenu.BlinkMenu;

            AbilityId = abilityId;
            Refresh();

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
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            blinkDagger = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId) as BlinkDagger;
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

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero.Hero) || args.IsQueued || manager.MyHero.IsChanneling || !args.Process
                || blinkDagger.IsSleeping)
            {
                return;
            }

            if (args.OrderId == OrderId.AbilityLocation && args.Ability?.Id == AbilityId)
            {
                var location = args.TargetPosition;
                var castRange = blinkDagger.GetCastRange();

                if (manager.MyHero.Distance2D(location) <= castRange)
                {
                    return;
                }

                args.Process = false;
                blinkDagger.Use(manager.MyHero.Position.Extend(location, castRange - 50));
            }
        }
    }
}
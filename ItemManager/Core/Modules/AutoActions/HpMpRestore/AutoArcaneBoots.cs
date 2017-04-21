namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    [AbilityBasedModule(AbilityId.item_arcane_boots)]
    internal class AutoArcaneBoots : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly AutoArcaneBootsMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private ArcaneBoots arcaneBoots;

        private bool notified;

        public AutoArcaneBoots(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoArcaneBootsMenu;

            Refresh();

            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.item_arcane_boots
        };

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        public void Refresh()
        {
            arcaneBoots = manager.MyHero.UsableAbilities.FirstOrDefault(x => x.Id == AbilityIds.First()) as ArcaneBoots;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero.Hero) || !args.Process || args.OrderId != OrderId.Ability)
            {
                return;
            }

            if (args.Ability?.Id == AbilityIds.First())
            {
                notified = false;
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(500);

            if (!menu.AutoUse || Game.IsPaused || !manager.MyHero.CanUseItems() || !arcaneBoots.CanBeCasted()
                || manager.MyHero.MissingMana < arcaneBoots.ManaRestore)
            {
                return;
            }

            if (ObjectManager.GetEntitiesParallel<Hero>()
                .Any(
                    x => x.IsValid && x.Handle != manager.MyHero.Handle && x.IsAlive && x.IsVisible && !x.IsIllusion
                         && x.Team == manager.MyHero.Team
                         && x.Distance2D(manager.MyHero.Position) <= menu.AllySearchRange
                         && x.Distance2D(manager.MyHero.Position) > arcaneBoots.GetCastRange()
                         && x.MaximumMana - x.Mana > arcaneBoots.ManaRestore))
            {
                if (!notified && menu.NotifyAllies)
                {
                    Network.ItemAlert(manager.MyHero.Position, AbilityIds.First());
                    DelayAction.Add(200, () => Network.ItemAlert(manager.MyHero.Position, AbilityIds.First()));
                    notified = true;
                }

                return;
            }

            arcaneBoots.Use();
        }
    }
}
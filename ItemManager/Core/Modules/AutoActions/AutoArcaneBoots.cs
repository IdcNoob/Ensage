namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Linq;

    using Abilities;

    using Attributes;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    [Module]
    internal class AutoArcaneBoots : IDisposable
    {
        private readonly Manager manager;

        private readonly AutoArcaneBootsMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private ArcaneBoots arcaneBoots;

        private bool notified;

        private bool subscribed;

        public AutoArcaneBoots(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoArcaneBootsMenu;

            manager.OnItemAdd += OnItemAdd;
            manager.OnItemRemove += OnItemRemove;
        }

        public void Dispose()
        {
            manager.OnItemAdd -= OnItemAdd;
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (!args.Entities.Contains(manager.MyHero) || !args.Process || args.OrderId != OrderId.Ability)
            {
                return;
            }

            if (args.Ability?.Id == AbilityId.item_arcane_boots)
            {
                notified = false;
            }
        }

        private void OnItemAdd(object sender, ItemEventArgs itemEventArgs)
        {
            if (!itemEventArgs.IsMine || itemEventArgs.Item.Id != AbilityId.item_arcane_boots)
            {
                return;
            }

            arcaneBoots =
                manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_arcane_boots) as ArcaneBoots;

            if (arcaneBoots == null || subscribed)
            {
                return;
            }

            subscribed = true;
            Game.OnUpdate += OnUpdate;
            Player.OnExecuteOrder += OnExecuteOrder;
        }

        private void OnItemRemove(object sender, ItemEventArgs itemEventArgs)
        {
            if (!itemEventArgs.IsMine || itemEventArgs.Item.Id != AbilityId.item_arcane_boots)
            {
                return;
            }

            arcaneBoots =
                manager.UsableAbilities.FirstOrDefault(x => x.Id == AbilityId.item_arcane_boots) as ArcaneBoots;

            if (arcaneBoots != null || !subscribed)
            {
                return;
            }

            subscribed = false;
            Game.OnUpdate -= OnUpdate;
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping || Game.IsPaused)
            {
                return;
            }

            sleeper.Sleep(500);

            if (!menu.AutoUse || !manager.MyHeroCanUseItems() || !arcaneBoots.CanBeCasted()
                || manager.MyMissingMana < arcaneBoots.ManaRestore)
            {
                return;
            }

            if (ObjectManager.GetEntitiesParallel<Hero>()
                .Any(
                    x => x.IsValid && x.Handle != manager.MyHandle && x.IsAlive && x.IsVisible && !x.IsIllusion
                         && x.Team == manager.MyTeam && x.Distance2D(manager.MyHero) <= menu.AllySearchRange
                         && x.Distance2D(manager.MyHero) > arcaneBoots.GetCastRange()
                         && x.MaximumMana - x.Mana > arcaneBoots.ManaRestore))
            {
                if (!notified && menu.NotifyAllies)
                {
                    Network.ItemAlert(manager.MyHero.Position, AbilityId.item_arcane_boots);
                    DelayAction.Add(200, () => Network.ItemAlert(manager.MyHero.Position, AbilityId.item_arcane_boots));
                    notified = true;
                }

                return;
            }

            arcaneBoots.Use();
        }
    }
}
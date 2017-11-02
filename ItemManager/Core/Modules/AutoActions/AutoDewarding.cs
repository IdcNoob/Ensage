namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;

    using EventArgs;

    using Menus;
    using Menus.Modules.AutoActions.Actions;

    using Utils;

    [Module]
    internal class AutoDewarding : IDisposable
    {
        private readonly float chopRange;

        private readonly Manager manager;

        private readonly DewardingMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private readonly IUpdateHandler updateHandler;

        public AutoDewarding(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.DewardingMenu;

            chopRange = Ability.GetAbilityDataById(AbilityId.item_quelling_blade)
                            .AbilitySpecialData.First(x => x.Name == "cast_range_ward")
                            .Value + 200;

            updateHandler = UpdateManager.Subscribe(OnUpdate, this.menu.UpdateRate, this.menu.IsEnabled);
            this.menu.OnEnabledChange += MenuOnEnabledChange;
            this.menu.OnUpdateRateChange += MenuOnUpdateRateChange;
        }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            menu.OnUpdateRateChange -= MenuOnUpdateRateChange;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            updateHandler.IsEnabled = boolEventArgs.Enabled;
        }

        private void MenuOnUpdateRateChange(object sender, IntEventArgs intEventArgs)
        {
            updateHandler.SetUpdateRate(intEventArgs.Time);
        }

        private void OnUpdate()
        {
            if (sleeper.Sleeping || Game.IsPaused || !manager.MyHero.CanUseItems())
            {
                return;
            }

            var item = manager.MyHero.GetItems(ItemStoredPlace.Inventory)
                .Where(
                    x => menu.ItemsToUse.Contains(x.Id) && (!x.IsTango() || manager.MyHero.MissingHealth >= menu.TangoHpThreshold)
                         && menu.IsAbilityEnabled(x.StoredName()) && x.CanBeCasted())
                .OrderByDescending(x => menu.GetAbilityPriority(x.StoredName()))
                .FirstOrDefault();

            if (item == null)
            {
                return;
            }

            var ward = EntityManager<Unit>.Entities
                .Where(
                    x => x.IsValid && x.IsVisible && x.IsWard() && x.IsAlive && x.Team != manager.MyHero.Team
                         && x.Distance2D(manager.MyHero.Position) <= chopRange)
                .OrderByDescending(x => x.IsSentryWard())
                .FirstOrDefault();

            if (ward == null)
            {
                return;
            }

            item.UseAbility(ward);
            sleeper.Sleep(500);
        }
    }
}
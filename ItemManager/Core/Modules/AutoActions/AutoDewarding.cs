namespace ItemManager.Core.Modules.AutoActions
{
    using System;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

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

        public AutoDewarding(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.DewardingMenu;

            chopRange = Ability.GetAbilityDataById(AbilityId.item_quelling_blade)
                .AbilitySpecialData.First(x => x.Name == "cast_range_ward")
                .Value;

            Game.OnUpdate += OnUpdate;
        }

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping || Game.IsPaused)
            {
                return;
            }

            sleeper.Sleep(200);

            if (!menu.IsEnabled || !manager.MyHero.CanUseItems())
            {
                return;
            }

            var ward = ObjectManager.GetEntitiesParallel<Unit>()
                .FirstOrDefault(
                    x => x.IsValid && x.IsWard() && x.IsAlive && x.Team != manager.MyHero.Team
                         && x.Distance2D(manager.MyHero.Position) <= chopRange);

            if (ward == null)
            {
                return;
            }

            var item = manager.MyHero.GetMyItems(ItemStoredPlace.Inventory)
                .Where(
                    x => (!x.IsTango() || manager.MyHero.MissingHealth >= menu.TangoHpThreshold)
                         && menu.IsAbilityEnabled(x.StoredName()) && x.CanBeCasted())
                .OrderByDescending(x => menu.GetAbilityPriority(x.StoredName()))
                .FirstOrDefault();

            item?.UseAbility(ward);
        }
    }
}
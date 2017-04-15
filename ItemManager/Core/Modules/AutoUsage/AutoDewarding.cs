namespace ItemManager.Core.Modules.AutoUsage
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus.Modules.AutoUsage;

    using Utils;

    internal class AutoDewarding : IDisposable
    {
        private readonly float chopRange;

        private readonly Manager manager;

        private readonly Deward menu;

        private readonly Sleeper sleeper = new Sleeper();

        public AutoDewarding(Manager manager, Deward menu)
        {
            this.manager = manager;
            this.menu = menu;

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

            if (!manager.MyHeroCanUseItems())
            {
                return;
            }

            var destroyableUnit = ObjectManager.GetEntitiesParallel<Unit>()
                .FirstOrDefault(
                    x => x.IsValid && (x.IsWard() || menu.DestroyMines && x.IsTechiesMine() && x.Health > 1)
                         && x.IsAlive && x.Team != manager.MyTeam && x.Distance2D(manager.MyHero) <= chopRange);

            if (destroyableUnit == null)
            {
                return;
            }

            var item = manager.GetMyItems(ItemUtils.StoredPlace.Inventory)
                .Where(
                    x => (!x.IsTango() || manager.MyMissingHealth >= menu.TangoHpThreshold
                          && !destroyableUnit.IsTechiesMine()) && menu.IsAbilityEnabled(x.StoredName())
                         && x.CanBeCasted())
                .OrderByDescending(x => menu.GetAbilityPriority(x.StoredName()))
                .FirstOrDefault();

            item?.UseAbility(destroyableUnit);
        }
    }
}
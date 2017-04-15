namespace ItemManager.Core.Modules.AutoUsage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using Menus.Modules.AutoUsage;

    using Utils;

    internal class AutoDewarding : IDisposable
    {
        private readonly float castRangeOnWards;

        private readonly Manager manager;

        private readonly Deward menu;

        private readonly Sleeper sleeper = new Sleeper();

        private readonly List<ClassId> wards = new List<ClassId>
        {
            ClassId.CDOTA_NPC_Observer_Ward,
            ClassId.CDOTA_NPC_Observer_Ward_TrueSight
        };

        public AutoDewarding(Manager manager, Deward menu)
        {
            this.manager = manager;
            this.menu = menu;

            castRangeOnWards = Ability.GetAbilityDataById(AbilityId.item_quelling_blade)
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

            var ward = ObjectManager.GetEntitiesParallel<Unit>()
                .FirstOrDefault(
                    x => x.IsValid && wards.Contains(x.ClassId) && x.IsAlive && x.Team != manager.MyTeam
                         && x.Distance2D(manager.MyHero) <= castRangeOnWards);

            if (ward == null)
            {
                return;
            }

            var item = manager.GetMyItems(ItemUtils.StoredPlace.Inventory)
                .Where(
                    x => (!x.IsTango() || manager.MyMissingHealth > menu.TangoHpThreshold)
                         && menu.IsAbilityEnabled(x.StoredName()) && x.CanBeCasted())
                .OrderByDescending(x => menu.GetAbilityPriority(x.StoredName()))
                .FirstOrDefault();

            item?.UseAbility(ward);
        }
    }
}
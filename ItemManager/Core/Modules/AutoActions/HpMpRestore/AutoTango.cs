namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    using EventArgs;

    using Interfaces;

    using Menus;
    using Menus.Modules.AutoActions.HpMpRestore;

    using Utils;

    [AbilityBasedModule(AbilityId.item_tango_single)]
    [AbilityBasedModule(AbilityId.item_tango)]
    internal class AutoTango : IAbilityBasedModule
    {
        private readonly Manager manager;

        private readonly AutoTangoMenu menu;

        private readonly Sleeper sleeper = new Sleeper();

        private float branchRemovalTime;

        public AutoTango(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoTangoMenu;

            Refresh();

            Game.OnUpdate += OnUpdate;
            manager.OnItemRemove += OnItemRemove;
        }

        public List<AbilityId> AbilityIds { get; } = new List<AbilityId>
        {
            AbilityId.item_tango,
            AbilityId.item_tango_single
        };

        public void Dispose()
        {
            manager.OnItemRemove -= OnItemRemove;
            Game.OnUpdate -= OnUpdate;
        }

        public void Refresh()
        {
        }

        private void OnItemRemove(object sender, ItemEventArgs itemEventArgs)
        {
            if (itemEventArgs.Item.Id == AbilityId.item_branches)
            {
                branchRemovalTime = Game.RawGameTime;
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(200);

            if (!menu.IsEnabled || !manager.MyHero.CanUseItems() || Game.IsPaused
                || manager.MyHero.MissingHealth < menu.HealthThreshold
                || manager.MyHero.HasModifier(ModifierUtils.TangoRegeneration)
                || branchRemovalTime + 10 < Game.RawGameTime)
            {
                return;
            }

            var happyKappaTree = ObjectManager.GetEntitiesParallel<Tree>()
                .FirstOrDefault(
                    x => x.IsValid && x.IsAlive && x.Distance2D(manager.MyHero.Position) < 300
                         && x.ClassId == ClassId.CDOTA_TempTree);

            if (happyKappaTree == null)
            {
                return;
            }

            var tango = manager.MyHero.GetMyItems(ItemStoredPlace.Inventory)
                .FirstOrDefault(x => x.IsTango() && x.CanBeCasted());

            tango?.UseAbility(happyKappaTree);
        }
    }
}
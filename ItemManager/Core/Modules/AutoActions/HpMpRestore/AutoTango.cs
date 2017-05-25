namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;

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

        private float branchRemovalTime;

        public AutoTango(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoTangoMenu;

            AbilityId = abilityId;
            Refresh();

            UpdateManager.Subscribe(OnUpdate, 500);
            manager.OnItemRemove += OnItemRemove;
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            manager.OnItemRemove -= OnItemRemove;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
        }

        private void OnItemRemove(object sender, ItemEventArgs itemEventArgs)
        {
            if (itemEventArgs.Item.Id == AbilityId.item_branches)
            {
                branchRemovalTime = Game.RawGameTime;
                UpdateManager.BeginInvoke(OnUpdate, 100);
            }
        }

        private void OnUpdate()
        {
            if (!menu.IsEnabled || branchRemovalTime + 10 < Game.RawGameTime || Game.IsPaused
                || manager.MyHero.MissingHealth < menu.HealthThreshold
                || manager.MyHero.HasModifier(ModifierUtils.TangoRegeneration) || !manager.MyHero.CanUseItems())
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

            var tango = manager.MyHero.GetItems(ItemStoredPlace.Inventory)
                .FirstOrDefault(x => x.IsTango() && x.CanBeCasted());

            tango?.UseAbility(happyKappaTree);
        }
    }
}
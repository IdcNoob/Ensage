namespace ItemManager.Core.Modules.AutoActions.HpMpRestore
{
    using System.Linq;

    using Attributes;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Handlers;
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

        private readonly IUpdateHandler updateHandler;

        public AutoTango(Manager manager, MenuManager menu, AbilityId abilityId)
        {
            this.manager = manager;
            this.menu = menu.AutoActionsMenu.AutoHealsMenu.AutoTangoMenu;

            AbilityId = abilityId;
            Refresh();

            updateHandler = UpdateManager.Subscribe(OnUpdate, 100, false);
            if (this.menu.IsEnabled)
            {
                manager.OnItemRemove += OnItemRemove;
            }
            this.menu.OnEnabledChange += MenuOnEnabledChange;
        }

        public AbilityId AbilityId { get; }

        public void Dispose()
        {
            menu.OnEnabledChange -= MenuOnEnabledChange;
            manager.OnItemRemove -= OnItemRemove;
            UpdateManager.Unsubscribe(OnUpdate);
        }

        public void Refresh()
        {
        }

        private void MenuOnEnabledChange(object sender, BoolEventArgs boolEventArgs)
        {
            if (boolEventArgs.Enabled)
            {
                manager.OnItemRemove += OnItemRemove;
            }
            else
            {
                manager.OnItemRemove -= OnItemRemove;
            }
        }

        private void OnItemRemove(object sender, ItemEventArgs itemEventArgs)
        {
            if (itemEventArgs.Item.Id == AbilityId.item_branches)
            {
                updateHandler.IsEnabled = true;
                UpdateManager.BeginInvoke(() => updateHandler.IsEnabled = false, 20000);
            }
        }

        private void OnUpdate()
        {
            if (Game.IsPaused || manager.MyHero.MissingHealth < menu.HealthThreshold
                || manager.MyHero.HasModifier(ModifierUtils.TangoRegeneration) || !manager.MyHero.CanUseItems())
            {
                return;
            }

            var happyKappaTree = EntityManager<Tree>.Entities.FirstOrDefault(
                x => x.IsValid && x.IsVisible && x.Distance2D(manager.MyHero.Position) < 300
                     && x.NetworkName == "CDOTA_TempTree");

            if (happyKappaTree == null)
            {
                return;
            }

            var tango = manager.MyHero.GetItems(ItemStoredPlace.Inventory).FirstOrDefault(x => x.IsTango() && x.CanBeCasted());

            if (tango == null)
            {
                return;
            }

            tango.UseAbility(happyKappaTree);
            updateHandler.IsEnabled = false;
        }
    }
}
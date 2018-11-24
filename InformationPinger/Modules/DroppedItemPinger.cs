namespace InformationPinger.Modules
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Service;

    using Interfaces;

    //[Export(typeof(IModule))]
    internal class DroppedItemPinger : IModule
    {
        private readonly IInformationPinger informationPinger;

        private readonly Team myTeam;

        private readonly IMenuManager rootMenu;

        private MenuItem<bool> doublePing;

        private MenuItem<bool> enabled;

        private MenuItem<AbilityToggler> pingedItems;

        [ImportingConstructor]
        public DroppedItemPinger(
            [Import] IServiceContext context,
            [Import] IMenuManager menu,
            [Import] IInformationPinger pinger)
        {
            myTeam = context.Owner.Team;
            rootMenu = menu;
            informationPinger = pinger;
        }

        public bool IsActive { get; private set; }

        public void Activate()
        {
            CreateMenu();

            if (enabled)
            {
                EntityManager<PhysicalItem>.EntityAdded += OnEntityAdded;
            }
            enabled.Item.ValueChanged += ItemOnValueChanged;
        }

        public void Dispose()
        {
            EntityManager<PhysicalItem>.EntityAdded -= OnEntityAdded;
        }

        private void CreateMenu()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            var menu = rootMenu.MenuFactory.Menu("Dropped items");
            enabled = menu.Item("Enabled", true);
            enabled.Item.SetTooltip("Ping dropped items");
            doublePing = menu.Item("Double ping", false);
            doublePing.Item.SetTooltip("Will ping items 2 times");
            pingedItems = menu.Item(
                "Force ping:",
                new AbilityToggler(
                    new Dictionary<string, bool>
                    {
                        { "item_gem", true },
                        { "item_rapier", true },
                    }));
        }

        private void ItemOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                EntityManager<PhysicalItem>.EntityAdded += OnEntityAdded;
            }
            else
            {
                EntityManager<PhysicalItem>.EntityAdded -= OnEntityAdded;
            }
        }

        private void OnEntityAdded(object sender, PhysicalItem item)
        {
            if (!item.IsValid || !pingedItems.Value.IsEnabled(item.Item.Name))
            {
                return;
            }

            if (EntityManager<Building>.Entities.Any(
                x => x.Team == myTeam && x.NetworkName == "CDOTA_Unit_Fountain"
                     && x.Position.Distance(item.Position) < 1000))
            {
                return;
            }

            //TODO how to ping physical item =/ 
        }
    }
}
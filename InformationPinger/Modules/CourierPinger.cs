namespace InformationPinger.Modules
{
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Service;

    using Interfaces;

    using PingTypes;

    [Export(typeof(IModule))]
    internal class CourierPinger : IModule
    {
        private readonly IInformationPinger informationPinger;

        private readonly Team myTeam;

        private readonly IMenuManager rootMenu;

        private MenuItem<Slider> delay;

        private MenuItem<bool> enabled;

        private MenuItem<bool> sayThanks;

        private MenuItem<bool> sayTime;

        private Sleeper sleeper;

        [ImportingConstructor]
        public CourierPinger(
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

            if (EntityManager<Courier>.Entities.Any(
                x => x.Team == myTeam && x.ClassId == ClassId.CDOTA_Unit_Courier && x.IsFlying))
            {
                return;
            }

            sleeper = new Sleeper();
            if (enabled)
            {
                UpdateManager.Subscribe(OnUpdate, 5000, enabled);
                Unit.OnModifierAdded += OnModifierAdded;
            }
            enabled.Item.ValueChanged += EnabledValueChanged;
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
            Unit.OnModifierAdded -= OnModifierAdded;
            enabled.Item.ValueChanged -= EnabledValueChanged;
        }

        private void CreateMenu()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            var menu = rootMenu.MenuFactory.Menu("Courier");
            enabled = menu.Item("Enabled", true);
            enabled.Item.SetTooltip("Upgrade courier reminder");
            sayThanks = menu.Item("Say thanks", true);
            sayThanks.Item.SetTooltip("Say thanks after upgrade");
            sayTime = menu.Item("Say time", true);
            delay = menu.Item("Delay (mins)", new Slider(5, 1, 20));
            delay.Item.SetTooltip("Delay between pings");
        }

        private void EnabledValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                UpdateManager.Subscribe(OnUpdate, 5000, enabled);
                Unit.OnModifierAdded += OnModifierAdded;
            }
            else
            {
                UpdateManager.Unsubscribe(OnUpdate);
                Unit.OnModifierAdded -= OnModifierAdded;
            }
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (args.Modifier.Name != "modifier_courier_flying" || sender.Team != myTeam)
            {
                return;
            }

            if (sleeper.Sleeping && sayThanks)
            {
                informationPinger.AddPing(new ChatWheelPing(ChatWheelMessage.Thanks));
            }

            Dispose();
        }

        private void OnUpdate()
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            var stockInfo = Game.StockInfo
                .FirstOrDefault(x => x.Team == myTeam && x.AbilityId == AbilityId.item_flying_courier)
                ?.StockCount;

            if (stockInfo <= 0)
            {
                return;
            }

            informationPinger.AddPing(new ChatWheelPing(ChatWheelMessage.Upgrade_Courier, sayTime));
            sleeper.Sleep(delay * 60 * 1000);
        }
    }
}
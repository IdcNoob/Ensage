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
    internal class WardPinger : IModule
    {
        private readonly IInformationPinger informationPinger;

        private readonly Unit myHero;

        private readonly Team myTeam;

        private readonly IMenuManager rootMenu;

        private MenuItem<Slider> delay;

        private MenuItem<bool> enabled;

        private MenuItem<bool> sayThanks;

        private Sleeper sleeper;

        private Sleeper thanksSleeper;

        [ImportingConstructor]
        public WardPinger(
            [Import] IServiceContext context,
            [Import] IMenuManager menu,
            [Import] IInformationPinger pinger)
        {
            myHero = context.Owner;
            myTeam = myHero.Team;
            rootMenu = menu;
            informationPinger = pinger;
        }

        public bool IsActive { get; private set; }

        public void Activate()
        {
            CreateMenu();

            sleeper = new Sleeper();
            thanksSleeper = new Sleeper();

            if (enabled)
            {
                UpdateManager.Subscribe(OnUpdate, 5000, enabled);
                EntityManager<Item>.EntityAdded += OnEntityAdded;
            }
            enabled.Item.ValueChanged += EnabledValueChanged;
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
            EntityManager<Item>.EntityAdded -= OnEntityAdded;
            enabled.Item.ValueChanged -= EnabledValueChanged;
        }

        private void CreateMenu()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            var menu = rootMenu.MenuFactory.Menu("Wards");
            enabled = menu.Item("Enabled", true);
            enabled.Item.SetTooltip("We need wards reminder");
            sayThanks = menu.Item("Say thanks", true);
            sayThanks.Item.SetTooltip("Say thanks after wards purchased");
            delay = menu.Item("Delay (mins)", new Slider(5, 1, 20));
            delay.Item.SetTooltip("Delay between pings");
        }

        private void EnabledValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                UpdateManager.Subscribe(OnUpdate, 5000, enabled);
                EntityManager<Item>.EntityAdded += OnEntityAdded;
            }
            else
            {
                UpdateManager.Unsubscribe(OnUpdate);
                EntityManager<Item>.EntityAdded -= OnEntityAdded;
            }
        }

        private void OnEntityAdded(object sender, Item item)
        {
            if (thanksSleeper.Sleeping || !sleeper.Sleeping || !sayThanks || !enabled)
            {
                return;
            }

            var owner = item.Owner as Unit;
            if (owner == null || !owner.IsValid || owner.IsIllusion || owner.Team != myTeam)
            {
                return;
            }

            if (!item.IsValid || item.Id != AbilityId.item_ward_observer
                || item.Purchaser?.Hero?.Handle == myHero.Handle)
            {
                return;
            }

            thanksSleeper.Sleep(30000);
            informationPinger.AddPing(new ChatWheelPing(ChatWheelMessage.Thanks));
        }

        private void OnUpdate()
        {
            if (sleeper.Sleeping || Game.GameTime < 300)
            {
                return;
            }

            var stockInfo = Game.StockInfo
                .FirstOrDefault(x => x.Team == myTeam && x.AbilityId == AbilityId.item_ward_observer)
                ?.StockCount;

            if (stockInfo <= 0)
            {
                return;
            }

            if (EntityManager<Unit>.Entities.Any(x => x.NetworkName == "CDOTA_NPC_Observer_Ward" && x.Team == myTeam))
            {
                return;
            }

            if (EntityManager<Unit>.Entities.Count(
                    x => x.NetworkName == "CDOTA_NPC_Treant_EyesInTheForest" && x.Team == myTeam) >= 5)
            {
                return;
            }

            informationPinger.AddPing(new ChatWheelPing(ChatWheelMessage.NeedWards));
            sleeper.Sleep(delay * 60 * 1000);
        }
    }
}
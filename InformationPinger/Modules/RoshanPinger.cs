namespace InformationPinger.Modules
{
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;

    using Interfaces;

    using PingTypes;

    [Export(typeof(IModule))]
    internal class RoshanPinger : IModule
    {
        private readonly IInformationPinger informationPinger;

        private readonly IMenuManager rootMenu;

        private MenuItem<bool> enabled;

        [ImportingConstructor]
        public RoshanPinger([Import] IMenuManager menu, [Import] IInformationPinger pinger)
        {
            rootMenu = menu;
            informationPinger = pinger;
        }

        public bool IsActive { get; private set; }

        public bool RoshanKilled { get; set; }

        public void Activate()
        {
            CreateMenu();

            if (enabled)
            {
                Game.OnFireEvent += Game_OnFireEvent;
            }
            enabled.Item.ValueChanged += ItemOnValueChanged;
        }

        public void Dispose()
        {
            Game.OnFireEvent -= Game_OnFireEvent;
            enabled.Item.ValueChanged -= ItemOnValueChanged;
        }

        private void CreateMenu()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            var menu = rootMenu.MenuFactory.Menu("Roshan");
            enabled = menu.Item("Enabled", true);
            enabled.Item.SetTooltip("Roshan death time");
        }

        private void Game_OnFireEvent(FireEventEventArgs args)
        {
            if (args.GameEvent.Name == "dota_roshan_kill")
            {
                UpdateManager.BeginInvoke(
                    () => { informationPinger.AddPing(new ChatWheelPing(ChatWheelMessage.Roshan, true)); },
                    1000);
            }
        }

        private void ItemOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                Game.OnFireEvent += Game_OnFireEvent;
            }
            else
            {
                Game.OnFireEvent -= Game_OnFireEvent;
            }
        }
    }
}
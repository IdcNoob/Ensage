namespace InformationPinger.Modules
{
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects.UtilityObjects;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;

    using Interfaces;

    using PingTypes;

    [Export(typeof(IModule))]
    internal class RunePinger : IModule
    {
        private readonly IInformationPinger informationPinger;

        private readonly IMenuManager rootMenu;

        private MenuItem<Slider> autoDisableTime;

        private MenuItem<bool> enabled;

        private MenuItem<bool> sayTime;

        private Sleeper sleeper;

        private MenuItem<Slider> time;

        private IUpdateHandler updateHandler;

        [ImportingConstructor]
        public RunePinger([Import] IMenuManager menu, [Import] IInformationPinger pinger)
        {
            rootMenu = menu;
            informationPinger = pinger;
        }

        public bool IsActive { get; private set; }

        public void Activate()
        {
            CreateMenu();

            sleeper = new Sleeper();
            updateHandler = UpdateManager.Subscribe(OnUpdate, 2000, enabled);
            enabled.Item.ValueChanged += ItemOnValueChanged;
        }

        public void Dispose()
        {
            UpdateManager.Unsubscribe(OnUpdate);
            enabled.Item.ValueChanged -= ItemOnValueChanged;
        }

        private void CreateMenu()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            var menu = rootMenu.MenuFactory.Menu("Runes");
            enabled = menu.Item("Enabled", true);
            enabled.Item.SetTooltip("Check runes reminder");
            sayTime = menu.Item("Say time", true);
            time = menu.Item("Time (secs)", new Slider(10, 5, 20));
            time.Item.SetTooltip("Seconds before rune spawn");
            autoDisableTime = menu.Item("Auto disable (mins)", new Slider(10, 4, 30));
            autoDisableTime.Item.SetTooltip("Auto disable after X mins");
        }

        private void ItemOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            updateHandler.IsEnabled = args.GetNewValue<bool>();
        }

        private void OnUpdate()
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            var gameTime = Game.GameTime;
            if (gameTime > autoDisableTime * 60)
            {
                Dispose();
            }

            if (gameTime % 120 > 120 - time)
            {
                informationPinger.AddPing(new ChatWheelPing(ChatWheelMessage.Check_Runes, sayTime));
                sleeper.Sleep(90 * 1000);
            }
        }
    }
}
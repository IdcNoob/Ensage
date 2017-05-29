namespace PauseControl
{
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("PauseControl", StartupMode.Auto, "IdcNoob")]
    internal class PauseControl : Plugin
    {
        private static Team heroTeam;

        private Settings settings;

        private IUpdateHandler updateHandler;

        [ImportingConstructor]
        public PauseControl([Import] IServiceContext context)
        {
            heroTeam = context.Owner.Team;
        }

        protected override void OnActivate()
        {
            settings = new Settings();

            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
            updateHandler = UpdateManager.Subscribe(OnUpdate, 5000, false);
        }

        protected override void OnDeactivate()
        {
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            UpdateManager.Unsubscribe(OnUpdate);

            settings.Dispose();
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (sender.ClassId != ClassId.CDOTAGamerulesProxy || args.PropertyName != "m_iPauseTeam")
            {
                return;
            }

            var pauseTeam = (Team)args.NewValue;
            if (pauseTeam == heroTeam || pauseTeam == Team.Observer)
            {
                updateHandler.IsEnabled = false;
                return;
            }

            updateHandler.IsEnabled = settings.Unpause;
        }

        private void OnRemoveEntity(EntityEventArgs args)
        {
            if (!settings.Pause)
            {
                return;
            }

            var player = args.Entity as Player;
            if (player != null && player.Team == heroTeam)
            {
                if (!Game.IsPaused)
                {
                    Network.PauseGame();
                }
                else if (updateHandler.IsEnabled)
                {
                    updateHandler.IsEnabled = false;
                }
            }
        }

        private void OnUpdate()
        {
            if (Game.IsPaused)
            {
                Network.PauseGame();
                return;
            }

            updateHandler.IsEnabled = false;
        }
    }
}
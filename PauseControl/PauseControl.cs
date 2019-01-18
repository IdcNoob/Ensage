namespace PauseControl
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Threading;
    using System.Threading.Tasks;

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

        private TaskHandler unpauseTask;

        [ImportingConstructor]
        public PauseControl([Import] IServiceContext context)
        {
            heroTeam = context.Owner.Team;
        }

        protected override void OnActivate()
        {
            this.settings = new Settings();

            if (this.settings.Pause)
            {
                EntityManager<Player>.EntityRemoved += this.EntityManagerOnEntityRemoved;
            }
            if (this.settings.Unpause)
            {
                Entity.OnInt32PropertyChange += this.OnInt32PropertyChange;
            }

            this.unpauseTask = new TaskHandler(this.Unpause, true);

            this.settings.Pause.PropertyChanged += this.PauseOnPropertyChanged;
            this.settings.Unpause.PropertyChanged += this.UnpauseOnPropertyChanged;
        }

        protected override void OnDeactivate()
        {
            this.settings.Pause.PropertyChanged -= this.PauseOnPropertyChanged;
            this.settings.Unpause.PropertyChanged -= this.UnpauseOnPropertyChanged;
            EntityManager<Player>.EntityRemoved -= this.EntityManagerOnEntityRemoved;
            Entity.OnInt32PropertyChange -= this.OnInt32PropertyChange;
            this.unpauseTask.Cancel();

            this.settings.Dispose();
        }

        private void EntityManagerOnEntityRemoved(object sender, Player player)
        {
            if (player?.Team != heroTeam)
            {
                return;
            }

            if (!Game.IsPaused)
            {
                Network.PauseGame();
            }
            else if (this.unpauseTask.IsRunning)
            {
                this.unpauseTask.Cancel();
            }
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.NewValue == args.OldValue || args.PropertyName != "m_iPauseTeam")
            {
                return;
            }

            var pauseTeam = (Team)args.NewValue;
            if (pauseTeam == heroTeam || pauseTeam == Team.Observer)
            {
                this.unpauseTask.Cancel();
                return;
            }

            this.unpauseTask.RunAsync();
        }

        private void PauseOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.settings.Pause)
            {
                EntityManager<Player>.EntityRemoved += this.EntityManagerOnEntityRemoved;
            }
            else
            {
                EntityManager<Player>.EntityRemoved -= this.EntityManagerOnEntityRemoved;
            }
        }

        private async Task Unpause(CancellationToken cancellationToken)
        {
            if (Game.IsPaused)
            {
                Network.PauseGame();
                await Task.Delay(5000, cancellationToken);
                return;
            }

            this.unpauseTask.Cancel();
        }

        private void UnpauseOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.settings.Unpause)
            {
                Entity.OnInt32PropertyChange += this.OnInt32PropertyChange;
            }
            else
            {
                Entity.OnInt32PropertyChange -= this.OnInt32PropertyChange;
            }
        }
    }
}
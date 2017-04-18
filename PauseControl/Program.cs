namespace PauseControl
{
    using System;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;

    internal static class Program
    {
        private static readonly Random Random = new Random();

        private static Team heroTeam;

        private static Menu menu;

        private static MenuItem pause;

        private static MenuItem unpause;

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            ObjectManager.OnRemoveEntity -= OnRemoveEntity;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;

            menu.RemoveFromMainMenu();
        }

        private static void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (sender.ClassId != ClassId.CDOTAGamerulesProxy || args.PropertyName != "m_iPauseTeam")
            {
                return;
            }

            var pauseTeam = (Team)args.NewValue;
            if (pauseTeam == heroTeam || pauseTeam == Team.Observer)
            {
                return;
            }

            if (unpause.IsActive())
            {
                Unpause();
            }
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            menu = new Menu("Pause Control", "pauseControl", true);

            menu.AddItem(
                pause = new MenuItem("enabledPause", "Auto pause").SetValue(true)
                    .SetTooltip("Auto pause when ally is disconnected"));
            menu.AddItem(
                unpause = new MenuItem("enabledUnpause", "Force unpause").SetValue(false)
                    .SetTooltip("Force unpause if set by enemies"));

            menu.AddToMainMenu();

            heroTeam = ObjectManager.LocalHero.Team;

            ObjectManager.OnRemoveEntity += OnRemoveEntity;
            Entity.OnInt32PropertyChange += OnInt32PropertyChange;
        }

        private static void OnRemoveEntity(EntityEventArgs args)
        {
            if (!pause.IsActive())
            {
                return;
            }

            var player = args.Entity as Player;
            if (player != null && player.Team == heroTeam && !Game.IsPaused)
            {
                Network.PauseGame();
            }
        }

        private static async void Unpause()
        {
            while (Game.IsPaused)
            {
                Network.PauseGame();
                await Task.Delay(Random.Next(1111, 1222));
            }
        }
    }
}
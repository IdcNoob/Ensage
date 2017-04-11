namespace PauseControl
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects;

    internal static class Program
    {
        private static Menu menu;

        private static readonly Random Random = new Random();

        private static Team heroTeam;

        private static CustomSleeper sleeper;

        private static void Game_OnIngameUpdate(EventArgs args)
        {
            if (sleeper.Sleeping)
            {
                return;
            }

            sleeper.Sleep(1000);

            var dcedAlly = Heroes.GetByTeam(heroTeam).Any(x => x.Player == null);

            if (Game.IsPaused)
            {
                if (menu.Item("enabledUnpause").GetValue<bool>()
                    && (menu.Item("ignoreAlly").GetValue<bool>() || !dcedAlly))
                {
                    Game.ExecuteCommand("dota_pause");
                    sleeper.Sleep(Random.Next(1111, 1222));
                }
            }
            else if (dcedAlly && menu.Item("enabledPause").GetValue<bool>()
                     && !menu.Item("ignoreAlly").GetValue<bool>())
            {
                Game.ExecuteCommand("dota_pause");
                sleeper.Sleep(Random.Next(3333, 4444));
            }
        }

        private static void Main()
        {
            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Game.OnIngameUpdate -= Game_OnIngameUpdate;
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            menu = new Menu("Pause Control", "pauseControl", true);

            menu.AddItem(new MenuItem("enabledPause", "Auto pause").SetValue(true));
            menu.AddItem(new MenuItem("enabledUnpause", "Auto unpause").SetValue(true));
            menu.AddItem(
                new MenuItem("ignoreAlly", "Ignore ally").SetValue(false)
                    .SetTooltip("Unpause game even when ally is disconnected"));

            menu.AddToMainMenu();

            heroTeam = ObjectManager.LocalHero.Team;
            sleeper = new CustomSleeper();
            Game.OnIngameUpdate += Game_OnIngameUpdate;
        }
    }
}
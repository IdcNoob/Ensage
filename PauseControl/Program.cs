namespace PauseControl
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;
    using Ensage.Common.Objects;

    internal class MySleeper
    {
        #region Fields

        private float lastSleepTickCount;

        #endregion

        #region Public Properties

        public bool Sleeping => (Environment.TickCount & int.MaxValue) < lastSleepTickCount;

        #endregion

        #region Public Methods and Operators

        public void Sleep(float duration)
        {
            lastSleepTickCount = (Environment.TickCount & int.MaxValue) + duration;
        }

        #endregion
    }

    internal static class Program
    {
        #region Static Fields

        private static readonly Menu Menu = new Menu("Pause Control", "pauseControl", true);

        private static readonly Random Random = new Random();

        private static Team heroTeam;

        private static MySleeper sleeper;

        #endregion

        #region Methods

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
                if (Menu.Item("enabledUnpause").GetValue<bool>()
                    && (Menu.Item("ignoreAlly").GetValue<bool>() || !dcedAlly))
                {
                    Game.ExecuteCommand("dota_pause");

                    //prevent const interval console spam, just in case...
                    sleeper.Sleep(Random.Next(1111, 1222));
                }
            }
            else if (dcedAlly && Menu.Item("enabledPause").GetValue<bool>() && !Menu.Item("ignoreAlly").GetValue<bool>())
            {
                Game.ExecuteCommand("dota_pause");

                //prevent const interval console spam, just in case...
                sleeper.Sleep(Random.Next(3333, 4444));
            }
        }

        private static void Main()
        {
            Menu.AddItem(new MenuItem("enabledPause", "Auto pause").SetValue(true));
            Menu.AddItem(new MenuItem("enabledUnpause", "Auto unpause").SetValue(true));
            Menu.AddItem(
                new MenuItem("ignoreAlly", "Ignore ally").SetValue(false)
                    .SetTooltip("Unpause game even when ally is disconnected"));

            Menu.AddToMainMenu();

            Events.OnLoad += OnLoad;
            Events.OnClose += OnClose;
        }

        private static void OnClose(object sender, EventArgs e)
        {
            Game.OnIngameUpdate -= Game_OnIngameUpdate;
        }

        private static void OnLoad(object sender, EventArgs e)
        {
            heroTeam = ObjectManager.LocalHero.Team;
            sleeper = new MySleeper();
            Game.OnIngameUpdate += Game_OnIngameUpdate;
        }

        #endregion
    }
}
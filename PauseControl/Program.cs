namespace PauseControl
{
    using System;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Menu;

    internal static class Program
    {
        #region Static Fields

        private static readonly Menu Menu = new Menu("Pause Control", "pauseControl", true);

        private static readonly Random Random = new Random();

        private static Team heroTeam;

        #endregion

        #region Methods

        private static void Game_OnIngameUpdate(EventArgs args)
        {
            if (!Utils.SleepCheck("PauseControl.Sleep"))
            {
                return;
            }

            Utils.Sleep(1000, "PauseControl.Sleep");

            if (heroTeam == Team.None)
            {
                return;
            }

            var dcedAlly = ObjectManager.GetEntities<Hero>().Any(x => x.Team == heroTeam && x.Player == null);

            if (Game.IsPaused)
            {
                if (Menu.Item("enabledUnpause").GetValue<bool>()
                    && (Menu.Item("ignoreAlly").GetValue<bool>() || !dcedAlly))
                {
                    Game.ExecuteCommand("dota_pause");

                    //prevent const interval console spam, just in case...
                    Utils.Sleep(Random.Next(1111, 1222), "PauseControl.Sleep");
                }
            }
            else if (dcedAlly && Menu.Item("enabledPause").GetValue<bool>() && !Menu.Item("ignoreAlly").GetValue<bool>())
            {
                Game.ExecuteCommand("dota_pause");

                //prevent const interval console spam, just in case...
                Utils.Sleep(Random.Next(3333, 4444), "PauseControl.Sleep");
            }
        }

        private static void Main()
        {
            Menu.AddItem(new MenuItem("enabledPause", "Auto pause").SetValue(true));
            Menu.AddItem(new MenuItem("enabledUnpause", "Auto unpause").SetValue(true));
            Menu.AddItem(new MenuItem("ignoreAlly", "Ignore ally").SetValue(false)
                .SetTooltip("Unpause game even when ally is disconnected"));

            Menu.AddToMainMenu();

            Events.OnLoad += delegate { heroTeam = ObjectManager.LocalPlayer.Team; };
            Events.OnClose += delegate { heroTeam = Team.None; };
            Game.OnIngameUpdate += Game_OnIngameUpdate;
        }

        #endregion
    }
}
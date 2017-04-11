namespace InformationPinger
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        private readonly InformationPinger pinger = new InformationPinger();

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            pinger.OnUpdate();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            pinger.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            pinger.OnLoad();
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
        }
    }
}
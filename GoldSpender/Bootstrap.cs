namespace GoldSpender
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly GoldSpender goldSpender = new GoldSpender();

        #endregion

        #region Public Methods and Operators

        public void Initialize()
        {
            Variables.MenuManager = new MenuManager();
            Events.OnLoad += OnLoad;
        }

        #endregion

        #region Methods

        private void Game_OnUpdate(EventArgs args)
        {
            goldSpender.OnUpdate();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            goldSpender.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            goldSpender.OnLoad();
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
        }

        #endregion
    }
}
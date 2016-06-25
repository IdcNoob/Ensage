namespace GoldSpender
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    internal class Bootstrap
    {
        #region Fields

        private readonly GoldSpender goldSpender = new GoldSpender();

        #endregion

        #region Public Methods and Operators

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
            Variables.MenuManager = new MenuManager();
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
        }

        private void OnLoad(object sender, EventArgs e)
        {
            Variables.Hero = ObjectManager.LocalHero;

            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
        }

        #endregion
    }
}
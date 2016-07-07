namespace PredictedCreepsLocation
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly Predictions predictions = new Predictions();

        #endregion

        #region Public Methods and Operators

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        #endregion

        #region Methods

        private void Drawing_OnDraw(EventArgs args)
        {
            predictions.OnDraw();
        }

        private void Game_OnClose(object sender, EventArgs e)
        {
            Drawing.OnDraw -= Drawing_OnDraw;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Events.OnClose -= Game_OnClose;
            ObjectManager.OnRemoveEntity -= ObjectManager_OnRemoveEntity;
            predictions.OnClose();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            predictions.OnUpdate();
        }

        private void ObjectManager_OnRemoveEntity(EntityEventArgs args)
        {
            predictions.OnRemoveEntity(args);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            predictions.OnLoad();
            ObjectManager.OnRemoveEntity += ObjectManager_OnRemoveEntity;
            Game.OnIngameUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Events.OnClose += Game_OnClose;
        }

        #endregion
    }
}
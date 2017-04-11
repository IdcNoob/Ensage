namespace PredictedCreepsLocation
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        private readonly Predictions predictions = new Predictions();

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            predictions.OnDraw();
        }

        private void Drawing_OnEndScene(EventArgs args)
        {
            predictions.DrawingOnEndScene();
        }

        private void Drawing_OnPostReset(EventArgs args)
        {
            predictions.OnPostReset();
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            predictions.OnPreReset();
        }

        private void Game_OnClose(object sender, EventArgs e)
        {
            Drawing.OnDraw -= Drawing_OnDraw;
            Drawing.OnPreReset -= Drawing_OnPreReset;
            Drawing.OnPostReset -= Drawing_OnPostReset;
            Drawing.OnEndScene -= Drawing_OnEndScene;
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
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }
    }
}
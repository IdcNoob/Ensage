namespace AbilityLastHitMarker
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        private readonly LastHitMarker lastHitMarker = new LastHitMarker();

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            lastHitMarker.OnDraw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            lastHitMarker.OnUpdate();
        }

        private void ObjectManager_OnAddEntity(EntityEventArgs args)
        {
            lastHitMarker.OnAddEntity(args);
        }

        private void ObjectManager_OnRemoveEntity(EntityEventArgs args)
        {
            lastHitMarker.OnRemoveEntity(args);
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            ObjectManager.OnAddEntity -= ObjectManager_OnAddEntity;
            ObjectManager.OnRemoveEntity -= ObjectManager_OnRemoveEntity;
            lastHitMarker.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            lastHitMarker.OnLoad();
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            ObjectManager.OnAddEntity += ObjectManager_OnAddEntity;
            ObjectManager.OnRemoveEntity += ObjectManager_OnRemoveEntity;
        }
    }
}
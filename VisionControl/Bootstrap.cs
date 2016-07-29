namespace VisionControl
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly VisionControl visionControl = new VisionControl();

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
            visionControl.OnDraw();
        }

        private void Entity_OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            visionControl.OnInt32PropertyChange(sender, args);
        }

        private void Game_OnUpdate(EventArgs args)
        {
            visionControl.OnUpdate();
        }

        private void ObjectManager_OnAddEntity(EntityEventArgs args)
        {
            visionControl.OnAddEntity(args);
        }

        private void ObjectManager_OnRemoveEntity(EntityEventArgs args)
        {
            visionControl.OnRemoveUnit(args.Entity as Unit);
        }

        private void OnClose(object sender, EventArgs e)
        {
            Drawing.OnDraw -= Drawing_OnDraw;
            Game.OnIngameUpdate -= Game_OnUpdate;
            ObjectManager.OnAddEntity -= ObjectManager_OnAddEntity;
            ObjectManager.OnRemoveEntity -= ObjectManager_OnRemoveEntity;
            Events.OnClose -= OnClose;
            //Entity.OnInt32PropertyChange -= Entity_OnInt32PropertyChange;
            visionControl.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            visionControl.OnLoad();
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            ObjectManager.OnAddEntity += ObjectManager_OnAddEntity;
            ObjectManager.OnRemoveEntity += ObjectManager_OnRemoveEntity;
            //Entity.OnInt32PropertyChange += Entity_OnInt32PropertyChange;
        }

        #endregion
    }
}
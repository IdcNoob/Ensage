namespace JungleStacker
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly JungleStacker jungleStacker;

        #endregion

        #region Constructors and Destructors

        public Bootstrap()
        {
            jungleStacker = new JungleStacker();
            Events.OnLoad += Events_OnLoad;
        }

        #endregion

        #region Methods

        private void Events_OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= Events_OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            ObjectManager.OnAddEntity -= ObjectManager_OnAddEntity;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            jungleStacker.OnClose();
        }

        private void Events_OnLoad(object sender, EventArgs e)
        {
            jungleStacker.OnLoad();
            Events.OnClose += Events_OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            ObjectManager.OnAddEntity += ObjectManager_OnAddEntity;
            ObjectManager.OnRemoveEntity += ObjectManager_OnRemoveEntity;
            Player.OnExecuteOrder += Player_OnExecuteAction;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            jungleStacker.OnUpdate();
        }

        private void ObjectManager_OnAddEntity(EntityEventArgs args)
        {
            jungleStacker.OnAddEntity(args);
        }

        private void ObjectManager_OnRemoveEntity(EntityEventArgs args)
        {
            jungleStacker.OnRemoveEntity(args);
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            jungleStacker.OnExecuteAction(args.Ability, args.Target);
        }

        #endregion
    }
}
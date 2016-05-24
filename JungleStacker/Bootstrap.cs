namespace JungleStacker
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly JungleStacker jungleStacking;

        #endregion

        #region Constructors and Destructors

        public Bootstrap()
        {
            this.jungleStacking = new JungleStacker();
            Events.OnLoad += this.Events_OnLoad;
        }

        #endregion

        #region Methods

        private void Events_OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= this.Events_OnClose;
            Game.OnIngameUpdate -= this.Game_OnUpdate;
            ObjectManager.OnAddEntity -= this.ObjectManager_OnAddEntity;
            Player.OnExecuteOrder -= this.Player_OnExecuteAction;
            this.jungleStacking.OnClose();
        }

        private void Events_OnLoad(object sender, EventArgs e)
        {
            this.jungleStacking.OnLoad();
            Events.OnClose += this.Events_OnClose;
            Game.OnIngameUpdate += this.Game_OnUpdate;
            ObjectManager.OnAddEntity += this.ObjectManager_OnAddEntity;
            ObjectManager.OnRemoveEntity += this.ObjectManager_OnRemoveEntity;
            Player.OnExecuteOrder += this.Player_OnExecuteAction;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            this.jungleStacking.OnUpdate();
        }

        private void ObjectManager_OnAddEntity(EntityEventArgs args)
        {
            this.jungleStacking.OnAddEntity(args);
        }

        private void ObjectManager_OnRemoveEntity(EntityEventArgs args)
        {
            this.jungleStacking.OnRemoveEntity(args);
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            this.jungleStacking.OnExecuteAction(args.Ability, args.Target);
        }

        #endregion
    }
}
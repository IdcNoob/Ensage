namespace HpMpAbuse
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly Abuse abuse = new Abuse();

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
            abuse.OnDraw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            abuse.OnUpdate();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            Drawing.OnDraw -= Drawing_OnDraw;
            abuse.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            abuse.OnLoad();
            Drawing.OnDraw += Drawing_OnDraw;
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += Player_OnExecuteAction;
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            abuse.OnExecuteAction(sender, args);
        }

        #endregion
    }
}
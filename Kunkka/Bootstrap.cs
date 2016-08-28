namespace Kunkka
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly Kunkka kunkka = new Kunkka();

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
            kunkka.OnDraw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            kunkka.OnUpdate();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            Drawing.OnDraw -= Drawing_OnDraw;
            kunkka.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (ObjectManager.LocalHero.ClassID != ClassID.CDOTA_Unit_Hero_Kunkka)
            {
                return;
            }

            kunkka.OnLoad();
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += Player_OnExecuteAction;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            kunkka.OnExecuteAbilitiy(sender, args);
        }

        #endregion
    }
}
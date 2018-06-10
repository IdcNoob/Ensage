namespace Timbersaw
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("Timbersaw?", HeroId.npc_dota_hero_shredder)]
    internal class Bootstrap : Plugin
    {
        private Timbersaw timbersaw;

        protected override void OnActivate()
        {
            timbersaw = new Timbersaw();

            timbersaw.OnLoad();
            Game.OnIngameUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += Player_OnExecuteAction;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        protected override void OnDeactivate()
        {
            Game.OnIngameUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            timbersaw.OnClose();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            timbersaw.OnDraw();
        }

        private void Game_OnUpdate(EventArgs args)
        {
            timbersaw.OnUpdate();
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            timbersaw.OnExecuteAbilitiy(sender, args);
        }
    }
}
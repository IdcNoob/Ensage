namespace JungleStacker
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("Jungle stacker")]
    internal class Bootstrap : Plugin
    {
        private JungleStacker jungleStacker;

        protected override void OnActivate()
        {
            jungleStacker = new JungleStacker();
            jungleStacker.OnLoad();
            Game.OnUpdate += Game_OnUpdate;
            ObjectManager.OnAddEntity += ObjectManager_OnAddEntity;
            ObjectManager.OnRemoveEntity += ObjectManager_OnRemoveEntity;
            Player.OnExecuteOrder += Player_OnExecuteAction;
        }

        protected override void OnDeactivate()
        {
            Game.OnUpdate -= Game_OnUpdate;
            ObjectManager.OnAddEntity -= ObjectManager_OnAddEntity;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            jungleStacker.OnClose();
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
            jungleStacker.OnExecuteAction(args);
        }
    }
}
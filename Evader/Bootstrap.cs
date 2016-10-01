namespace Evader
{
    using System;

    using Core;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        #region Fields

        private readonly Evader evader = new Evader();

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
            evader.OnDraw();
        }

        private void Entity_OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            evader.OnParticleEffectAdded(sender, args);
        }

        private void Game_OnUpdate(EventArgs args)
        {
            evader.OnUpdate();
        }

        private void ObjectManager_OnAddEntity(EntityEventArgs args)
        {
            evader.OnAddEntity(args);
        }

        private void ObjectManagerOnAddTrackingProjectile(TrackingProjectileEventArgs args)
        {
            evader.OnAddTrackingProjectile(args);
        }

        private void ObjectManagerOnRemoveEntity(EntityEventArgs args)
        {
            evader.OnRemoveEntity(args.Entity);
        }

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            Drawing.OnDraw -= Drawing_OnDraw;
            ObjectManager.OnRemoveEntity -= ObjectManagerOnRemoveEntity;
            ObjectManager.OnAddEntity -= ObjectManager_OnAddEntity;
            Unit.OnModifierAdded -= Unit_OnModifierAdded;
            Entity.OnParticleEffectAdded -= Entity_OnParticleEffectAdded;
            ObjectManager.OnAddTrackingProjectile -= ObjectManagerOnAddTrackingProjectile;

            evader.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            evader.OnLoad();
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += Player_OnExecuteAction;
            Drawing.OnDraw += Drawing_OnDraw;
            ObjectManager.OnRemoveEntity += ObjectManagerOnRemoveEntity;
            ObjectManager.OnAddEntity += ObjectManager_OnAddEntity;
            Unit.OnModifierAdded += Unit_OnModifierAdded;
            Entity.OnParticleEffectAdded += Entity_OnParticleEffectAdded;
            ObjectManager.OnAddTrackingProjectile += ObjectManagerOnAddTrackingProjectile;
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            evader.OnExecuteAction(sender, args);
        }

        private void Unit_OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            evader.OnModifierAdded(sender, args.Modifier);
        }

        #endregion
    }
}
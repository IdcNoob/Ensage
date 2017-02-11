namespace Evader.Core
{
    using System;

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

        private void OnClose(object sender, EventArgs e)
        {
            Events.OnClose -= OnClose;
            Game.OnUpdate -= Game_OnUpdate;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            Drawing.OnDraw -= Drawing_OnDraw;
            ObjectManager.OnAddEntity -= ObjectManager_OnAddEntity;
            Unit.OnModifierAdded -= Unit_OnModifierAdded;
            Unit.OnModifierRemoved -= Unit_OnModifierRemoved;
            Entity.OnParticleEffectAdded -= Entity_OnParticleEffectAdded;
            evader.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            evader.OnLoad();

            Events.OnClose += OnClose;
            Game.OnUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += Player_OnExecuteAction;
            Drawing.OnDraw += Drawing_OnDraw;
            ObjectManager.OnAddEntity += ObjectManager_OnAddEntity;
            Unit.OnModifierAdded += Unit_OnModifierAdded;
            Unit.OnModifierRemoved += Unit_OnModifierRemoved;
            Entity.OnParticleEffectAdded += Entity_OnParticleEffectAdded;
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            evader.OnExecuteAction(sender, args);
        }

        private void Unit_OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            evader.OnModifierAdded(sender, args.Modifier);
        }

        private void Unit_OnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            evader.OnModifierRemoved(args.Modifier);
        }

        #endregion
    }
}
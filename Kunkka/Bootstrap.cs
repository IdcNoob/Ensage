namespace Kunkka
{
    using System;

    using Ensage;
    using Ensage.Common;

    internal class Bootstrap
    {
        private readonly Kunkka kunkka = new Kunkka();

        public void Initialize()
        {
            Events.OnLoad += OnLoad;
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            kunkka.OnDraw();
        }

        private void Entity_OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            kunkka.OnParticleEffectAdded(sender, args);
        }

        private void Game_OnUpdate(EventArgs args)
        {
            kunkka.OnUpdate();
        }

        private void OnClose(object sender, EventArgs e)
        {
            Entity.OnParticleEffectAdded -= Entity_OnParticleEffectAdded;
            Events.OnClose -= OnClose;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Player.OnExecuteOrder -= Player_OnExecuteAction;
            Drawing.OnDraw -= Drawing_OnDraw;
            Unit.OnModifierAdded -= UnitOnModifierAdded;
            Unit.OnModifierRemoved -= UnitOnModifierRemoved;
            kunkka.OnClose();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (ObjectManager.LocalHero.HeroId != HeroId.npc_dota_hero_kunkka)
            {
                return;
            }

            kunkka.OnLoad();
            Events.OnClose += OnClose;
            Game.OnIngameUpdate += Game_OnUpdate;
            Player.OnExecuteOrder += Player_OnExecuteAction;
            Drawing.OnDraw += Drawing_OnDraw;
            Entity.OnParticleEffectAdded += Entity_OnParticleEffectAdded;
            Unit.OnModifierAdded += UnitOnModifierAdded;
            Unit.OnModifierRemoved += UnitOnModifierRemoved;
        }

        private void Player_OnExecuteAction(Player sender, ExecuteOrderEventArgs args)
        {
            kunkka.OnExecuteAbility(sender, args);
        }

        private void UnitOnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            kunkka.OnModifierAdded(sender, args);
        }

        private void UnitOnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            kunkka.OnModifierRemoved(sender, args);
        }
    }
}
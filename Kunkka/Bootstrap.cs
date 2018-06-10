namespace Kunkka
{
    using System;

    using Ensage;
    using Ensage.Common;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("Kunkka helper", HeroId.npc_dota_hero_kunkka)]
    internal class Bootstrap : Plugin
    {
        private Kunkka kunkka = new Kunkka();

        protected override void OnActivate()
        {
            kunkka = new Kunkka();

            kunkka.OnLoad();
            Game.OnIngameUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Entity.OnParticleEffectAdded += Entity_OnParticleEffectAdded;
            Unit.OnModifierAdded += UnitOnModifierAdded;
            Unit.OnModifierRemoved += UnitOnModifierRemoved;
        }

        protected override void OnDeactivate()
        {
            Entity.OnParticleEffectAdded -= Entity_OnParticleEffectAdded;
            Game.OnIngameUpdate -= Game_OnUpdate;
            Drawing.OnDraw -= Drawing_OnDraw;
            Unit.OnModifierAdded -= UnitOnModifierAdded;
            Unit.OnModifierRemoved -= UnitOnModifierRemoved;
            kunkka.OnClose();
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
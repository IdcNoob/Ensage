namespace SimpleSD
{
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("SD", HeroId.npc_dota_hero_shadow_demon)]
    internal class SimplePlugin : Plugin
    {
        private readonly IServiceContext context;

        private OrbwalkingMode orbwalkingMode;

        private Settings settings;

        [ImportingConstructor]
        public SimplePlugin(IServiceContext context)
        {
            this.context = context;
        }

        protected override void OnActivate()
        {
            this.settings = new Settings(this.context.Owner.Name);
            this.orbwalkingMode = new OrbwalkingMode(this.context, this.settings);
            this.context.Orbwalker.RegisterMode(this.orbwalkingMode);
        }

        protected override void OnDeactivate()
        {
            this.context.Orbwalker.UnregisterMode(this.orbwalkingMode);
            this.settings.Dispose();
        }
    }
}
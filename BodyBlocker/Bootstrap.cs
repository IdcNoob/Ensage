namespace BodyBlocker
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Ensage.SDK.Menu;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using Modes;

    [ExportPlugin("Body Blocker", StartupMode.Auto, "IdcNoob")]
    internal class Bootstrap : Plugin
    {
        private MenuFactory factory;

        [ImportMany]
        private IEnumerable<IBodyBlockMode> modes;

        protected override void OnActivate()
        {
            factory = MenuFactory.Create("Body Blocker", "bodyBlocker");

            foreach (var mode in modes)
            {
                mode.Activate();
            }
        }

        protected override void OnDeactivate()
        {
            foreach (var mode in modes)
            {
                mode.Dispose();
            }

            factory.Dispose();
        }
    }
}
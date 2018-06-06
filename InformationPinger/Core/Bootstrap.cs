namespace InformationPinger.Core
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Ensage.Common.Menu;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using Interfaces;

    [ExportPlugin("Information Pinger", StartupMode.Auto, "IdcNoob")]
    internal class Bootstrap : Plugin
    {
        private readonly IMenuManager menu;

        private readonly IEnumerable<IModule> modules;

        [ImportingConstructor]
        public Bootstrap([ImportMany] IEnumerable<IModule> modules, [Import] IMenuManager menu)
        {
            this.modules = modules;
            this.menu = menu;
        }

        protected override void OnActivate()
        {
            menu.Activate();
            menu.Enabled.Item.ValueChanged += EnabledValueChanged;

            if (menu.Enabled)
            {
                ActivateModules();
            }
        }

        protected override void OnDeactivate()
        {
            DisposeModules();
            menu.Dispose();
        }

        private void ActivateModules()
        {
            foreach (var module in modules)
            {
                module.Activate();
            }
        }

        private void DisposeModules()
        {
            foreach (var module in modules)
            {
                module.Dispose();
            }
        }

        private void EnabledValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                ActivateModules();
            }
            else
            {
                DisposeModules();
            }
        }
    }
}
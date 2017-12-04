namespace Debugger
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using Tools;

    [ExportPlugin("Debugger", StartupMode.Auto, priority: 1)]
    internal class Bootstrap : Plugin
    {
        [ImportMany]
        private IEnumerable<IDebuggerTool> tools;

        protected override void OnActivate()
        {
            if (!Game.IsLobbyGame)
            {
                return;
            }

            foreach (var service in this.tools.OrderByDescending(x => x.LoadPriority))
            {
                service.Activate();
            }
        }

        protected override void OnDeactivate()
        {
            foreach (var service in this.tools.OrderBy(x => x.LoadPriority))
            {
                service.Dispose();
            }
        }
    }
}
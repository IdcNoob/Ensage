namespace ItemManager
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using Attributes;

    using Core;

    using Ensage;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using Menus;

    [ExportPlugin("Item Manager", StartupMode.Auto, "IdcNoob")]
    internal class Bootstrap : Plugin
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        private readonly Hero hero;

        [ImportingConstructor]
        public Bootstrap([Import] IServiceContext context)
        {
            hero = context.Owner as Hero;
        }

        protected override void OnActivate()
        {
            var manager = new Manager(hero);
            var menu = new MenuManager();

            disposables.Add(manager);
            disposables.Add(menu);

            foreach (var type in Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.Namespace?.Contains("ItemManager.Core.Modules") == true && x.GetCustomAttribute<ModuleAttribute>() != null))
            {
                disposables.Add((IDisposable)Activator.CreateInstance(type, manager, menu));
            }
        }

        protected override void OnDeactivate()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }

            disposables.Clear();
        }
    }
}
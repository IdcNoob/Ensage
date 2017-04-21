namespace ItemManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Attributes;

    using Core;

    using Menus;

    internal class Bootstrap
    {
        private readonly List<IDisposable> disposables = new List<IDisposable>();

        public void OnClose()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }

            disposables.Clear();
        }

        public void OnLoad()
        {
            var manager = new Manager();
            var menu = new MenuManager();

            disposables.Add(manager);
            disposables.Add(menu);

            foreach (var type in Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(
                    x => x.Namespace?.Contains("ItemManager.Core.Modules") == true
                         && x.GetCustomAttribute<ModuleAttribute>() != null))
            {
                disposables.Add((IDisposable)Activator.CreateInstance(type, manager, menu));
            }
        }
    }
}
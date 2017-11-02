namespace ItemManager.Core.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Attributes;

    using Ensage.SDK.Extensions;

    using EventArgs;

    using Interfaces;

    using Menus;

    [Module]
    internal class AbilityBasedModuleManager : IDisposable
    {
        private readonly Manager manager;

        private readonly MenuManager menu;

        private readonly List<IAbilityBasedModule> modules = new List<IAbilityBasedModule>();

        private readonly HashSet<Type> types;

        public AbilityBasedModuleManager(Manager manager, MenuManager menu)
        {
            this.manager = manager;
            this.menu = menu;

            types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.Namespace?.Contains("ItemManager.Core.Modules") == true)
                .ToHashSet();

            manager.OnAbilityAdd += OnAbilityAdd;
            manager.OnAbilityRemove += OnAbilityRemove;
        }

        public void Dispose()
        {
            manager.OnAbilityAdd -= OnAbilityAdd;
            manager.OnAbilityRemove -= OnAbilityRemove;

            foreach (var disposable in modules)
            {
                disposable.Dispose();
            }

            modules.Clear();
        }

        private void OnAbilityAdd(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine)
            {
                return;
            }

            var module = modules.FirstOrDefault(x => x.AbilityId == abilityEventArgs.Ability.Id);
            if (module != null)
            {
                module.Refresh();
                return;
            }

            var type = types.FirstOrDefault(
                x => x.GetCustomAttributes<AbilityBasedModuleAttribute>().Any(z => z.AbilityId == abilityEventArgs.Ability.Id));

            if (type != null)
            {
                modules.Add((IAbilityBasedModule)Activator.CreateInstance(type, manager, menu, abilityEventArgs.Ability.Id));
            }
        }

        private void OnAbilityRemove(object sender, AbilityEventArgs abilityEventArgs)
        {
            if (!abilityEventArgs.IsMine)
            {
                return;
            }

            var module = modules.FirstOrDefault(x => x.AbilityId == abilityEventArgs.Ability.Id);
            if (module == null)
            {
                return;
            }

            if (manager.MyHero.Abilities.Any(x => module.AbilityId == x.Id))
            {
                module.Refresh();
                return;
            }

            module.Dispose();
            modules.Remove(module);
        }
    }
}
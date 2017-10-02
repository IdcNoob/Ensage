namespace Debugger.Tools.OnAddRemove
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Abilities : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "special_",
            "_empty",
            "_hidden"
        };

        private MenuItem<bool> addEnabled;

        private MenuItem<bool> heroesOnly;

        private MenuItem<bool> ignoreUseless;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        private MenuItem<bool> removeEnabled;

        public int LoadPriority { get; } = 97;

        public void Activate()
        {
            this.menu = this.mainMenu.OnAddRemoveMenu.Menu("Abilities");

            this.addEnabled = this.menu.Item("On add enabled", false);
            this.addEnabled.Item.SetTooltip("EntityManager<Ability>.EntityAdded");
            this.addEnabled.PropertyChanged += this.AddEnabledPropertyChanged;

            this.removeEnabled = this.menu.Item("On remove enabled", false);
            this.removeEnabled.Item.SetTooltip("EntityManager<Ability>.EntityRemoved");
            this.removeEnabled.PropertyChanged += this.RemoveEnabledOnPropertyChanged;

            this.heroesOnly = this.menu.Item("Heroes only", false);
            this.ignoreUseless = this.menu.Item("Ignore useless", true);

            this.AddEnabledPropertyChanged(null, null);
            this.RemoveEnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.addEnabled.PropertyChanged -= this.AddEnabledPropertyChanged;
            this.removeEnabled.PropertyChanged -= this.RemoveEnabledOnPropertyChanged;
            EntityManager<Ability>.EntityAdded -= this.EntityManagerOnEntityAdded;
            EntityManager<Ability>.EntityRemoved -= this.EntityManagerOnEntityRemoved;
        }

        private void AddEnabledPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.addEnabled)
            {
                this.menu.AddAsterisk();
                EntityManager<Ability>.EntityAdded += this.EntityManagerOnEntityAdded;
            }
            else
            {
                if (!this.removeEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                EntityManager<Ability>.EntityAdded -= this.EntityManagerOnEntityAdded;
            }
        }

        private void EntityManagerOnEntityAdded(object sender, Ability ability)
        {
            if (!this.IsValid(ability))
            {
                return;
            }

            var item = new LogItem(LogType.Ability, Color.LightGreen, "Ability added");

            item.AddLine("Name: " + ability.Name, ability.Name);
            item.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
            item.AddLine("ClassID: " + ability.ClassId, ability.ClassId);
            item.AddLine("Owner name: " + ability.Owner.Name, ability.Owner.Name);
            item.AddLine("Owner network name: " + ability.Owner.NetworkName, ability.Owner.NetworkName);
            item.AddLine("Owner classID: " + ability.Owner.ClassId, ability.Owner.ClassId);

            this.log.Display(item);
        }

        private void EntityManagerOnEntityRemoved(object sender, Ability ability)
        {
            if (!this.IsValid(ability))
            {
                return;
            }

            var item = new LogItem(LogType.Ability, Color.LightPink, "Ability removed");

            item.AddLine("Name: " + ability.Name, ability.Name);
            item.AddLine("Network name: " + ability.NetworkName, ability.NetworkName);
            item.AddLine("ClassID: " + ability.ClassId, ability.ClassId);
            item.AddLine("Owner name: " + ability.Owner.Name, ability.Owner.Name);
            item.AddLine("Owner network name: " + ability.Owner.NetworkName, ability.Owner.NetworkName);
            item.AddLine("Owner classID: " + ability.Owner.ClassId, ability.Owner.ClassId);

            this.log.Display(item);
        }

        private bool IsValid(Entity entity)
        {
            if (entity?.IsValid != true)
            {
                return false;
            }

            if (this.ignoreUseless && this.ignored.Contains(entity.Name))
            {
                return false;
            }

            if (this.heroesOnly && !(entity.Owner is Hero))
            {
                return false;
            }

            return true;
        }

        private void RemoveEnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.removeEnabled)
            {
                this.menu.AddAsterisk();
                EntityManager<Ability>.EntityRemoved += this.EntityManagerOnEntityRemoved;
            }
            else
            {
                if (!this.addEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                EntityManager<Ability>.EntityRemoved -= this.EntityManagerOnEntityRemoved;
            }
        }
    }
}
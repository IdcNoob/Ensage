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

    internal class Units : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "portrait_world_unit"
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

        public int LoadPriority { get; } = 100;

        public void Activate()
        {
            this.menu = this.mainMenu.OnAddRemoveMenu.Menu("Units");

            this.addEnabled = this.menu.Item("On add enabled", false);
            this.addEnabled.Item.SetTooltip("EntityManager<Unit>.EntityAdded");
            this.addEnabled.PropertyChanged += this.AddEnabledPropertyChanged;

            this.removeEnabled = this.menu.Item("On remove enabled", false);
            this.removeEnabled.Item.SetTooltip("EntityManager<Unit>.EntityRemoved");
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
            EntityManager<Unit>.EntityAdded -= this.EntityManagerOnEntityAdded;
            EntityManager<Unit>.EntityRemoved -= this.EntityManagerOnEntityRemoved;
        }

        private void AddEnabledPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.addEnabled)
            {
                this.menu.AddAsterisk();
                EntityManager<Unit>.EntityAdded += this.EntityManagerOnEntityAdded;
            }
            else
            {
                if (!this.removeEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                EntityManager<Unit>.EntityAdded -= this.EntityManagerOnEntityAdded;
            }
        }

        private void EntityManagerOnEntityAdded(object sender, Unit unit)
        {
            if (!this.IsValid(unit))
            {
                return;
            }

            var item = new LogItem(LogType.Unit, Color.LightGreen, "Unit added");

            item.AddLine("Name: " + unit.Name, unit.Name);
            item.AddLine("Network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("ClassID: " + unit.ClassId, unit.ClassId);
            item.AddLine("Position: " + unit.Position, unit.Position);
            item.AddLine("Attack capability: " + unit.AttackCapability, unit.AttackCapability);
            item.AddLine("Move capability: " + unit.MoveCapability, unit.MoveCapability);
            item.AddLine("Vision: " + unit.DayVision + "/" + unit.NightVision, unit.DayVision + "/" + unit.NightVision);
            item.AddLine("Health: " + unit.Health, unit.Health);

            this.log.Display(item);
        }

        private void EntityManagerOnEntityRemoved(object sender, Unit unit)
        {
            if (!this.IsValid(unit))
            {
                return;
            }

            var item = new LogItem(LogType.Unit, Color.LightPink, "Unit removed");

            item.AddLine("Name: " + unit.Name, unit.Name);
            item.AddLine("Network name: " + unit.NetworkName, unit.NetworkName);
            item.AddLine("ClassID: " + unit.ClassId, unit.ClassId);
            item.AddLine("Position: " + unit.Position, unit.Position);

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

            if (this.heroesOnly && !(entity is Hero))
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
                EntityManager<Unit>.EntityRemoved += this.EntityManagerOnEntityRemoved;
            }
            else
            {
                if (!this.addEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                EntityManager<Unit>.EntityRemoved -= this.EntityManagerOnEntityRemoved;
            }
        }
    }
}
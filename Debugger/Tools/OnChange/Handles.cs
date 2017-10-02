namespace Debugger.Tools.OnChange
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Handles : IDebuggerTool
    {
        private MenuItem<bool> enabled;

        private MenuItem<bool> heroesOnly;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        public int LoadPriority { get; } = 92;

        public void Activate()
        {
            this.menu = this.mainMenu.OnChangeMenu.Menu("Handles");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Entity.OnHandlePropertyChange");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.heroesOnly = this.menu.Item("Heroes only", false);

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Entity.OnHandlePropertyChange -= this.OnHandlePropertyChange;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Entity.OnHandlePropertyChange += this.OnHandlePropertyChange;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Entity.OnHandlePropertyChange -= this.OnHandlePropertyChange;
            }
        }

        private bool IsValid(Entity sender, HandlePropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return false;
            }

            if (this.heroesOnly && !(sender is Hero) && !(sender.Owner is Hero))
            {
                return false;
            }

            return true;
        }

        private void OnHandlePropertyChange(Entity sender, HandlePropertyChangeEventArgs args)
        {
            if (!this.IsValid(sender, args))
            {
                return;
            }

            var item = new LogItem(LogType.Handle, Color.Cyan, "Handle changed");

            item.AddLine("Property name: " + args.PropertyName, args.PropertyName);
            item.AddLine("Property values: " + args.OldValue?.Name + " => " + args.NewValue?.Name, args.NewValue?.Name);
            item.AddLine("Sender name: " + sender.Name, sender.Name);
            item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
            item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);
            if (sender.Owner is Unit)
            {
                item.AddLine("Owner name: " + sender.Owner.Name, sender.Owner.Name);
                item.AddLine("Owner network name: " + sender.Owner.NetworkName, sender.Owner.NetworkName);
                item.AddLine("Owner classID: " + sender.Owner.ClassId, sender.Owner.ClassId);
            }

            this.log.Display(item);
        }
    }
}
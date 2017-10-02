namespace Debugger.Tools.OnChange
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Strings : IDebuggerTool
    {
        private MenuItem<bool> enabled;

        private MenuItem<bool> heroesOnly;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        public int LoadPriority { get; } = 89;

        public void Activate()
        {
            this.menu = this.mainMenu.OnChangeMenu.Menu("Strings");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Entity.OnStringPropertyChange");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.heroesOnly = this.menu.Item("Heroes only", false);

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Entity.OnStringPropertyChange -= this.OnStringPropertyChange;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Entity.OnStringPropertyChange += this.OnStringPropertyChange;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Entity.OnStringPropertyChange -= this.OnStringPropertyChange;
            }
        }

        private bool IsValid(Entity sender, StringPropertyChangeEventArgs args)
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

        private void OnStringPropertyChange(Entity sender, StringPropertyChangeEventArgs args)
        {
            if (!this.IsValid(sender, args))
            {
                return;
            }

            var item = new LogItem(LogType.String, Color.Cyan, "String changed");

            item.AddLine("Property name: " + args.PropertyName, args.PropertyName);
            item.AddLine("Property values: " + args.OldValue + " => " + args.NewValue, args.NewValue);
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
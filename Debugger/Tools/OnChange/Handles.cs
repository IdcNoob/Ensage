namespace Debugger.Tools.OnChange
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Handles : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "m_hModifierParent",
            "m_hModel",
            "m_hEffectEntity",
            "m_hParent"
        };

        private readonly HashSet<string> semiIgnored = new HashSet<string>
        {
            "m_hOwnerEntity",
            "m_hTowerAttackTarget"
        };

        private MenuItem<bool> enabled;

        private MenuItem<bool> heroesOnly;

        private MenuItem<bool> ignoreSemiUseless;

        private MenuItem<bool> ignoreUseless;

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
            this.ignoreSemiUseless = this.menu.Item("Ignore semi useless", false);
            this.ignoreUseless = this.menu.Item("Ignore useless", true);

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

            if (this.ignoreUseless && this.ignored.Contains(args.PropertyName))
            {
                return false;
            }

            if (this.ignoreSemiUseless && this.semiIgnored.Contains(args.PropertyName))
            {
                return false;
            }

            if (this.heroesOnly && !(sender is Hero) && !(sender.Owner is Hero))
            {
                return false;
            }

            return true;
        }

        private async void OnHandlePropertyChange(Entity sender, HandlePropertyChangeEventArgs args)
        {
            await Task.Delay(1);

            if (!this.IsValid(sender, args))
            {
                return;
            }

            var item = new LogItem(LogType.Handle, Color.Cyan, "Handle changed");

            item.AddLine("Property name: " + args.PropertyName, args.PropertyName);
            item.AddLine(
                "Property values: "
                + ObjectManager.GetEntities<Entity>().FirstOrDefault(x => x.IsValid && x.Index == args.OldValue.Index)?.Name + " => "
                + ObjectManager.GetEntities<Entity>().FirstOrDefault(x => x.IsValid && x.Index == args.NewValue.Index)?.Name,
                ObjectManager.GetEntities<Entity>().FirstOrDefault(x => x.IsValid && x.Index == args.NewValue.Index)?.Name);
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
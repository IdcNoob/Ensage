namespace Debugger.Tools.OnChange
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Bools : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "m_bIsMoving",
            "m_bHidden",
            "m_bIsGeneratingEconItem",
            "m_bInitialized",
            "m_bCanBeDominated",
            "m_bSelectionRingVisible",
            "m_bActivated",
            "m_bSellable",
            "m_bKillable",
            "m_bPurchasable",
            "m_bDroppable",
            "m_bCombinable",
            "m_bStackable",
            "m_bValid"
        };

        private MenuItem<bool> enabled;

        private MenuItem<bool> heroesOnly;

        private MenuItem<bool> ignoreUseless;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        public int LoadPriority { get; } = 94;

        public void Activate()
        {
            this.menu = this.mainMenu.OnChangeMenu.Menu("Bools");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Entity.OnBoolPropertyChange");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.heroesOnly = this.menu.Item("Heroes only", false);
            this.ignoreUseless = this.menu.Item("Ignore useless", true);

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Entity.OnBoolPropertyChange -= this.OnBoolPropertyChange;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Entity.OnBoolPropertyChange += this.OnBoolPropertyChange;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Entity.OnBoolPropertyChange -= this.OnBoolPropertyChange;
            }
        }

        private bool IsValid(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (args.OldValue.Equals(args.NewValue))
            {
                return false;
            }

            if (this.ignoreUseless && this.ignored.Contains(args.PropertyName))
            {
                return false;
            }

            if (this.heroesOnly && !(sender is Hero) && !(sender.Owner is Hero))
            {
                return false;
            }

            return true;
        }

        private void OnBoolPropertyChange(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (!this.IsValid(sender, args))
            {
                return;
            }

            var item = new LogItem(LogType.Bool, Color.Cyan, "Bool changed");

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
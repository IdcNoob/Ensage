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

    internal class Floats : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "m_vecX",
            "m_vecY",
            "m_vecZ",
            "m_fGameTime",
            "m_flMana",
            "m_flStartSequenceCycle",
            "m_flPlaybackRate",
            "m_flLastSpawnTime",
            "m_flMusicOperatorVals",
            "m_fStuns",
            "m_vecMins",
            "m_vecMaxs"
        };

        private readonly HashSet<string> semiIgnored = new HashSet<string>
        {
            "m_fCooldown",
            "m_flCooldownLength",
            "m_flPurchaseTime",
            "m_flAssembledTime",
            "m_flRadarCooldowns",
            "m_flElasticity",
            "m_flScale"
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

        public int LoadPriority { get; } = 93;

        public void Activate()
        {
            this.menu = this.mainMenu.OnChangeMenu.Menu("Floats");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Entity.OnFloatPropertyChange");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.heroesOnly = this.menu.Item("Heroes only", false);
            this.ignoreSemiUseless = this.menu.Item("Ignore semi useless", false);
            this.ignoreUseless = this.menu.Item("Ignore useless", true);

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Entity.OnFloatPropertyChange -= this.OnFloatPropertyChange;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Entity.OnFloatPropertyChange += this.OnFloatPropertyChange;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Entity.OnFloatPropertyChange -= this.OnFloatPropertyChange;
            }
        }

        private bool IsValid(Entity sender, FloatPropertyChangeEventArgs args)
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

        private void OnFloatPropertyChange(Entity sender, FloatPropertyChangeEventArgs args)
        {
            if (!this.IsValid(sender, args))
            {
                return;
            }

            var item = new LogItem(LogType.Float, Color.Cyan, "Float changed");

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
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

    internal class Int32 : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "m_iFoWFrameNumber",
            "m_iNetTimeOfDay",
            "m_iNetWorth",
            "m_iIncomeGold",
            "m_iTotalEarnedGold",
            "m_iUnreliableGold",
            "m_iReliableGold",
            "m_nHealthBarOffsetOverride",
            "m_nServerOrderSequenceNumber",
            "m_nResetEventsParity",
            "m_cellY",
            "m_cellX",
            "m_cellZ",
            "m_nGridY",
            "m_nGridX",
            "m_nGridZ",
            "m_nameStringableIndex",
            "m_iUnitNameIndex",
            "m_nIdealMotionType",
            "m_nDebugIndex",
            "m_NetworkSequenceIndex",
            "m_nNewSequenceParity",
            "m_iMusicOperatorVals",
            "m_anglediff",
            "m_iTaggedAsVisibleByTeam",
            "iStockCount"
        };

        private readonly HashSet<string> semiIgnored = new HashSet<string>
        {
            "m_iHealth",
            "m_iMaxHealth",
            "m_NetworkActivity",
            "m_iHeroDamage",
            "m_iRecentDamage",
            "m_iDamageBonus",
            "m_iMoveSpeed",
            "m_iDayTimeVisionRange",
            "m_iNightTimeVisionRange",
            "m_iPauseTeam",
            "m_iAttackCapabilities"
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

        public int LoadPriority { get; } = 91;

        public void Activate()
        {
            this.menu = this.mainMenu.OnChangeMenu.Menu("Int32");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Entity.OnInt32PropertyChange");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.heroesOnly = this.menu.Item("Heroes only", false);
            this.ignoreSemiUseless = this.menu.Item("Ignore semi useless", false);
            this.ignoreUseless = this.menu.Item("Ignore useless", true);

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Entity.OnInt32PropertyChange -= this.OnInt32PropertyChange;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Entity.OnInt32PropertyChange += this.OnInt32PropertyChange;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Entity.OnInt32PropertyChange -= this.OnInt32PropertyChange;
            }
        }

        private bool IsValid(Entity sender, Int32PropertyChangeEventArgs args)
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

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (!this.IsValid(sender, args))
            {
                return;
            }

            var item = new LogItem(LogType.Int32, Color.Cyan, "Int32 changed");

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
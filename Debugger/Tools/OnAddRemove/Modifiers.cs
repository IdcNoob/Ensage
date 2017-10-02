namespace Debugger.Tools.OnAddRemove
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Modifiers : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "modifier_projectile_vision",
            "modifier_truesight",
            "modifier_creep_haste",
            "modifier_creep_slow",
            "modifier_tower_aura",
            "modifier_tower_truesight_aura"
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

        public int LoadPriority { get; } = 99;

        public void Activate()
        {
            this.menu = this.mainMenu.OnAddRemoveMenu.Menu("Modifiers");

            this.addEnabled = this.menu.Item("On add enabled", false);
            this.addEnabled.Item.SetTooltip("Unit.OnModifierAdded");
            this.addEnabled.PropertyChanged += this.AddEnabledPropertyChanged;

            this.removeEnabled = this.menu.Item("On remove enabled", false);
            this.removeEnabled.Item.SetTooltip("Unit.OnModifierRemoved");
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
            Unit.OnModifierAdded -= this.UnitOnModifierAdded;
            Unit.OnModifierRemoved -= this.UnitOnModifierRemoved;
        }

        private void AddEnabledPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.addEnabled)
            {
                this.menu.AddAsterisk();
                Unit.OnModifierAdded += this.UnitOnModifierAdded;
            }
            else
            {
                if (!this.removeEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                Unit.OnModifierAdded -= this.UnitOnModifierAdded;
            }
        }

        private bool IsValid(Entity sender, Modifier modifier)
        {
            if (sender?.IsValid != true)
            {
                return false;
            }

            if (this.heroesOnly && !(sender is Hero))
            {
                return false;
            }

            if (modifier?.IsValid != true)
            {
                return false;
            }

            if (this.ignoreUseless && this.ignored.Contains(modifier.Name))
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
                Unit.OnModifierRemoved += this.UnitOnModifierRemoved;
            }
            else
            {
                if (!this.addEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                Unit.OnModifierRemoved -= this.UnitOnModifierRemoved;
            }
        }

        private async void UnitOnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            await Task.Delay(1);
            var modifier = args.Modifier;

            if (!this.IsValid(sender, modifier))
            {
                return;
            }

            var item = new LogItem(LogType.Modifier, Color.LightGreen, "Modifier added");

            item.AddLine("Name: " + modifier.Name, modifier.Name);
            item.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);
            item.AddLine("Elapsed time: " + modifier.ElapsedTime, modifier.ElapsedTime);
            item.AddLine("Remaining time: " + modifier.RemainingTime, modifier.RemainingTime);
            item.AddLine("Sender name: " + sender.Name, sender.Name);
            item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
            item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

            this.log.Display(item);
        }

        private void UnitOnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            var modifier = args.Modifier;

            if (!this.IsValid(sender, modifier))
            {
                return;
            }

            var item = new LogItem(LogType.Modifier, Color.LightPink, "Modifier removed");

            item.AddLine("Name: " + modifier.Name, modifier.Name);
            item.AddLine("Texture name: " + modifier.TextureName, modifier.TextureName);
            item.AddLine("Sender name: " + sender.Name, sender.Name);
            item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
            item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

            this.log.Display(item);
        }
    }
}
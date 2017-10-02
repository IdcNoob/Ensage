namespace Debugger.Tools.OnChange
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Animations : IDebuggerTool
    {
        private MenuItem<bool> enabled;

        private MenuItem<bool> heroesOnly;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        public int LoadPriority { get; } = 95;

        public void Activate()
        {
            this.menu = this.mainMenu.OnChangeMenu.Menu("Animations");

            this.enabled = this.menu.Item("Enabled", false);
            this.enabled.Item.SetTooltip("Entity.OnAnimationChanged");
            this.enabled.PropertyChanged += this.EnabledOnPropertyChanged;

            this.heroesOnly = this.menu.Item("Heroes only", false);

            this.EnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            Entity.OnAnimationChanged -= this.EntityOnAnimationChanged;
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.enabled)
            {
                this.menu.AddAsterisk();
                Entity.OnAnimationChanged += this.EntityOnAnimationChanged;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Entity.OnAnimationChanged -= this.EntityOnAnimationChanged;
            }
        }

        private void EntityOnAnimationChanged(Entity sender, EventArgs args)
        {
            if (!this.IsValid(sender))
            {
                return;
            }

            var item = new LogItem(LogType.Animation, Color.Cyan, "Animation changed");

            item.AddLine("Name: " + sender.Animation.Name, sender.Animation.Name);
            item.AddLine("Sender name: " + sender.Name, sender.Name);
            item.AddLine("Sender network name: " + sender.NetworkName, sender.NetworkName);
            item.AddLine("Sender classID: " + sender.ClassId, sender.ClassId);

            this.log.Display(item);
        }

        private bool IsValid(Entity sender)
        {
            if (this.heroesOnly && !(sender is Hero))
            {
                return false;
            }

            return true;
        }
    }
}
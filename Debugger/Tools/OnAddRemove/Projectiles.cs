namespace Debugger.Tools.OnAddRemove
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Menu;

    using Logger;

    using Menu;

    using SharpDX;

    internal class Projectiles : IDebuggerTool
    {
        private MenuItem<bool> addEnabled;

        private MenuItem<bool> heroesOnly;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        private MenuItem<bool> removeEnabled;

        public int LoadPriority { get; } = 96;

        public void Activate()
        {
            this.menu = this.mainMenu.OnAddRemoveMenu.Menu("Projectiles");

            this.addEnabled = this.menu.Item("On add enabled", false);
            this.addEnabled.Item.SetTooltip("ObjectManager.OnAddTrackingProjectile");
            this.addEnabled.PropertyChanged += this.AddEnabledPropertyChanged;

            this.removeEnabled = this.menu.Item("On remove enabled", false);
            this.removeEnabled.Item.SetTooltip("ObjectManager.OnRemoveTrackingProjectile");
            this.removeEnabled.PropertyChanged += this.RemoveEnabledOnPropertyChanged;

            this.heroesOnly = this.menu.Item("Heroes only", false);

            this.AddEnabledPropertyChanged(null, null);
            this.RemoveEnabledOnPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.addEnabled.PropertyChanged -= this.AddEnabledPropertyChanged;
            this.removeEnabled.PropertyChanged -= this.RemoveEnabledOnPropertyChanged;
            ObjectManager.OnRemoveTrackingProjectile -= this.OnRemoveTrackingProjectile;
            ObjectManager.OnAddTrackingProjectile -= this.OnTrackingProjectileAdded;
        }

        private void AddEnabledPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.addEnabled)
            {
                this.menu.AddAsterisk();
                ObjectManager.OnAddTrackingProjectile += this.OnTrackingProjectileAdded;
            }
            else
            {
                if (!this.removeEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                ObjectManager.OnAddTrackingProjectile -= this.OnTrackingProjectileAdded;
            }
        }

        private bool IsValid(TrackingProjectile projectile)
        {
            if (this.heroesOnly && !(projectile.Source is Hero))
            {
                return false;
            }

            return true;
        }

        private void OnRemoveTrackingProjectile(TrackingProjectileEventArgs args)
        {
            var projectile = args.Projectile;

            if (!this.IsValid(projectile))
            {
                return;
            }

            var item = new LogItem(LogType.Projectile, Color.LightPink, "Projectile removed");

            item.AddLine("Source name: " + projectile.Source?.Name, projectile.Source?.Name);
            item.AddLine("Source network name: " + projectile.Source?.NetworkName, projectile.Source?.NetworkName);
            item.AddLine("Source classID: " + projectile.Source?.ClassId, projectile.Source?.ClassId);
            item.AddLine("Speed: " + projectile.Speed, projectile.Speed);
            item.AddLine("Position: " + projectile.Position, projectile.Position);
            item.AddLine("Target name: " + projectile.Target?.Name, projectile.Target?.Name);
            item.AddLine("Target network name: " + projectile.Target?.NetworkName, projectile.Target?.NetworkName);
            item.AddLine("Target classID: " + projectile.Target?.ClassId, projectile.Target?.ClassId);
            item.AddLine("Target position: " + projectile.TargetPosition, projectile.TargetPosition);

            this.log.Display(item);
        }

        private void OnTrackingProjectileAdded(TrackingProjectileEventArgs args)
        {
            var projectile = args.Projectile;

            if (!this.IsValid(projectile))
            {
                return;
            }

            var item = new LogItem(LogType.Projectile, Color.LightGreen, "Projectile added");

            item.AddLine("Source name: " + projectile.Source?.Name, projectile.Source?.Name);
            item.AddLine("Source network name: " + projectile.Source?.NetworkName, projectile.Source?.NetworkName);
            item.AddLine("Source classID: " + projectile.Source?.ClassId, projectile.Source?.ClassId);
            item.AddLine("Speed: " + projectile.Speed, projectile.Speed);
            item.AddLine("Position: " + projectile.Position, projectile.Position);
            item.AddLine("Target name: " + projectile.Target?.Name, projectile.Target?.Name);
            item.AddLine("Target network name: " + projectile.Target?.NetworkName, projectile.Target?.NetworkName);
            item.AddLine("Target classID: " + projectile.Target?.ClassId, projectile.Target?.ClassId);
            item.AddLine("Target position: " + projectile.TargetPosition, projectile.TargetPosition);

            this.log.Display(item);
        }

        private void RemoveEnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.removeEnabled)
            {
                this.menu.AddAsterisk();
                ObjectManager.OnRemoveTrackingProjectile += this.OnRemoveTrackingProjectile;
            }
            else
            {
                if (!this.addEnabled)
                {
                    this.menu.RemoveAsterisk();
                }

                ObjectManager.OnRemoveTrackingProjectile -= this.OnRemoveTrackingProjectile;
            }
        }
    }
}
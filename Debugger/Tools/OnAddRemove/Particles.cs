namespace Debugger.Tools.OnAddRemove
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

    internal class Particles : IDebuggerTool
    {
        private readonly HashSet<string> ignored = new HashSet<string>
        {
            "ui_mouseactions",
            "generic_hit_blood",
            "base_attacks",
            "generic_gameplay",
            "ensage_ui"
        };

        private MenuItem<bool> addEnabled;

        private MenuItem<bool> ignoreUseless;

        private MenuItem<bool> ignoreZeroCp;

        [Import]
        private ILog log;

        [Import]
        private IMenu mainMenu;

        private MenuFactory menu;

        private MenuItem<bool> showCpValues;

        public int LoadPriority { get; } = 98;

        public void Activate()
        {
            this.menu = this.mainMenu.OnAddRemoveMenu.Menu("Particles");

            this.addEnabled = this.menu.Item("On add enabled", false);
            this.addEnabled.Item.SetTooltip("Entity.OnParticleEffectAdded");
            this.addEnabled.PropertyChanged += this.AddEnabledPropertyChanged;

            this.showCpValues = this.menu.Item("Show CP values", false);
            this.ignoreZeroCp = this.menu.Item("Ignore zero CP values", true);
            this.ignoreUseless = this.menu.Item("Ignore useless", true);

            this.AddEnabledPropertyChanged(null, null);
        }

        public void Dispose()
        {
            this.addEnabled.PropertyChanged -= this.AddEnabledPropertyChanged;
            Entity.OnParticleEffectAdded -= this.EntityOnParticleEffectAdded;
        }

        private void AddEnabledPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.addEnabled)
            {
                this.menu.AddAsterisk();
                Entity.OnParticleEffectAdded += this.EntityOnParticleEffectAdded;
            }
            else
            {
                this.menu.RemoveAsterisk();
                Entity.OnParticleEffectAdded -= this.EntityOnParticleEffectAdded;
            }
        }

        private async void EntityOnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            await Task.Delay(1);
            var particle = args.ParticleEffect;

            if (!this.IsValid(sender, particle, args.Name))
            {
                return;
            }

            var item = new LogItem(LogType.Particle, Color.LightGreen, "Particle added");

            item.AddLine("Name: " + args.Name, args.Name);
            item.AddLine("Highest control point: " + particle.HighestControlPoint, particle.HighestControlPoint);

            if (this.showCpValues)
            {
                for (var i = 0u; i <= args.ParticleEffect.HighestControlPoint; i++)
                {
                    var point = args.ParticleEffect.GetControlPoint(i);
                    if (this.ignoreZeroCp && point.IsZero)
                    {
                        continue;
                    }

                    item.AddLine("CP " + i + ": " + point, point);
                }
            }

            this.log.Display(item);
        }

        private bool IsValid(Entity sender, ParticleEffect particle, string name)
        {
            if (sender?.IsValid != true)
            {
                return false;
            }

            if (particle?.IsValid != true)
            {
                return false;
            }

            if (this.ignoreUseless && this.ignored.Any(name.Contains))
            {
                return false;
            }

            return true;
        }
    }
}
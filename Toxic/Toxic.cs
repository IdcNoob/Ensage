namespace Toxic
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("TOXIC", StartupMode.Auto, "IdcNoob")]
    internal class Toxic : Plugin
    {
        private Settings settings;

        protected override void OnActivate()
        {
            settings = new Settings();
            UpdateManager.BeginInvoke(OnUpdate);
        }

        protected override void OnDeactivate()
        {
            settings.Dispose();
        }

        private async void OnUpdate()
        {
            while (IsActive)
            {
                try
                {
                    if (settings.Enabled && !Game.IsPaused)
                    {
                        foreach (var hero in EntityManager<Hero>.Entities.Where(
                            x => x.IsValid && settings.Heroes.Value.IsEnabled(x.Name)))
                        {
                            Network.MapPing(
                                hero.Position.ToVector2(),
                                settings.DangerPing ? PingType.Danger : PingType.Normal);
                            await Task.Delay(25);
                        }
                    }

                    await Task.Delay(25);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
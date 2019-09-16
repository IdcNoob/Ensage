namespace PredictedCreepsLocation.Core
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Renderer;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using SharpDX;

    using Color = System.Drawing.Color;

    [ExportPlugin("Predicted Creeps Location", StartupMode.Auto, "IdcNoob")]
    internal class Prediction : Plugin
    {
        private readonly IRenderManager renderer;

        private readonly IWaveManager waveManager;

        private Settings settings;

        [ImportingConstructor]
        public Prediction([Import] IRenderManager renderer, [Import] IWaveManager waveManager)
        {
            this.renderer = renderer;
            this.waveManager = waveManager;
        }

        protected override void OnActivate()
        {
            settings = new Settings();
            waveManager.Activate();

            renderer.Draw += OnDraw;
            UpdateManager.Subscribe(OnUpdate, 500);
            UpdateManager.Subscribe(PositionUpdater);
        }

        protected override void OnDeactivate()
        {
            renderer.Draw -= OnDraw;
            UpdateManager.Unsubscribe(OnUpdate);
            UpdateManager.Unsubscribe(PositionUpdater);

            waveManager.Dispose();
            settings.Dispose();
        }

        private void OnDraw(IRenderer renderer1)
        {
            if (!settings.AlwaysEnabled && !settings.ShowKey)
            {
                return;
            }

            foreach (var wave in waveManager.CreepWaves.ToList().Where(x => x.IsSpawned && !x.IsVisible))
            {
                var text = settings.ShowCreepsCount ? wave.Creeps.Count.ToString() : "C";

                if (settings.ShowOnMinimap)
                {
                    renderer1.DrawText(wave.PredictedPosition.WorldToMinimap() - new Vector2(1, 15), text, Color.DarkOrange, 16, "Arial");
                }

                if (settings.ShowOnMap)
                {
                    var position = Drawing.WorldToScreen(wave.PredictedPosition);
                    if (position.IsZero)
                    {
                        continue;
                    }

                    renderer1.DrawText(position, text, Color.DarkOrange, 25, "Arial");
                }
            }
        }

        private void OnUpdate()
        {
            waveManager.CreepWaves.RemoveAll(x => x.IsSpawned && x.Path.Last().Distance(x.PredictedPosition) < 300);

            foreach (var group in waveManager.CreepWaves.Where(x => x.IsSpawned).GroupBy(x => x.Lane).Where(x => x.Count() > 1).ToList())
            {
                foreach (var wave in group)
                {
                    var merge = group.FirstOrDefault(x => !x.Equals(wave) && x.PredictedPosition.Distance(wave.PredictedPosition) < 500);

                    if (merge == null)
                    {
                        continue;
                    }

                    wave.Creeps.AddRange(merge.Creeps);
                    waveManager.CreepWaves.Remove(merge);
                    break;
                }
            }
        }

        private void PositionUpdater()
        {
            foreach (var wave in waveManager.CreepWaves.Where(x => x.IsSpawned))
            {
                if (!wave.WasVisible)
                {
                    wave.PredictedPosition = wave.Path.PositionAfter(Game.RawGameTime - wave.SpawnTime, wave.Speed, wave.Delay);
                }
                else if (wave.IsVisible)
                {
                    wave.PredictedPosition = wave.Position;
                }
                else
                {
                    wave.PredictedPosition = wave.RemainingPath.PositionAfter(
                        Game.RawGameTime - wave.LastVisibleTime,
                        wave.Speed,
                        wave.Delay);
                }
            }
        }
    }
}
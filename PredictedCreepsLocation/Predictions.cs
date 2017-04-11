namespace PredictedCreepsLocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;
    using SharpDX.Direct3D9;

    internal class Predictions
    {
        private readonly List<CreepWave> creepWaves = new List<CreepWave>();

        private readonly MenuManager menuManager = new MenuManager();

        private CreepsData creepsData;

        private Hero hero;

        private Team heroTeam;

        private MultiSleeper sleeper;

        private Font textFont;

        public void DrawingOnEndScene()
        {
            if (!menuManager.ShowOnMinimapEnabled)
            {
                return;
            }
            foreach (var wave in creepWaves.Where(x => !x.IsVisible()))
            {
                var minimapSize = menuManager.ShowOnMinimapSize;
                var position = wave.CurrentPosition.WorldToMinimap();
                if (position.IsZero)
                {
                    return;
                }

                position -= new Vector2(minimapSize) / 2;

                textFont.DrawText(null, "C", (int)position.X, (int)position.Y, Color.Orange);
            }
        }

        public void OnClose()
        {
            creepWaves.Clear();
        }

        public void OnDraw()
        {
            var gameTime = Game.GameTime;

            foreach (var wave in creepWaves)
            {
                var visibleAll = wave.IsVisible(true);

                if (!visibleAll)
                {
                    if (wave.WasVisible)
                    {
                        wave.PreviousPoint = wave.CurrentPosition;
                        wave.WasVisible = false;
                    }

                    wave.CurrentPosition = wave.PreviousPoint.Extend(
                        wave.NextPoint,
                        wave.GetSpeed() * (gameTime - wave.Time));
                }
                else
                {
                    var alive = wave.Creeps.Where(x => x.IsValid && x.IsAlive).ToList();

                    var middleCreep = alive.OrderByDescending(x => x.Distance2D(wave.NextPoint))
                        .ElementAtOrDefault(alive.Count / 2);

                    wave.Time = gameTime;

                    if (middleCreep != null)
                    {
                        wave.CurrentPosition = middleCreep.Position;
                        wave.WasVisible = true;
                    }
                }

                if (!IsPointOnLine(wave.CurrentPosition, wave.PreviousPoint, wave.NextPoint, visibleAll ? 500 : 10))
                {
                    if (wave.IgnorePoint)
                    {
                        wave.IgnorePoint = false;
                        continue;
                    }

                    var count = wave.LaneData.Points.Count;

                    if (count > ++wave.PointIndex)
                    {
                        wave.Time = gameTime;
                        wave.PreviousPoint = wave.CurrentPosition;
                        wave.NextPoint = wave.LaneData.Points.ElementAt(wave.PointIndex);
                    }
                    else
                    {
                        continue;
                    }
                }

                if (wave.IsVisible())
                {
                    continue;
                }

                if (menuManager.ShowOnMapEnabled)
                {
                    var mapSize = menuManager.ShowOnMapSize;
                    Vector2 position;
                    Drawing.WorldToScreen(wave.CurrentPosition, out position);

                    Drawing.DrawText(
                        "C",
                        "Arial",
                        position - new Vector2(0, mapSize),
                        new Vector2(mapSize),
                        Color.Orange,
                        FontFlags.None);
                }
            }
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            heroTeam = hero.Team;
            textFont = new Font(
                Drawing.Direct3DDevice9,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = menuManager.ShowOnMinimapSize,
                    OutputPrecision = FontPrecision.Raster,
                    Quality = FontQuality.ClearTypeNatural,
                    CharacterSet = FontCharacterSet.Hangul,
                    MipLevels = 3,
                    PitchAndFamily = FontPitchAndFamily.Modern,
                    Weight = FontWeight.Medium,
                    Width = menuManager.ShowOnMinimapSize / 2
                });
            creepsData = new CreepsData(heroTeam);
            sleeper = new MultiSleeper();
        }

        public void OnPostReset()
        {
            textFont.OnResetDevice();
        }

        public void OnPreReset()
        {
            textFont.OnLostDevice();
        }

        public void OnRemoveEntity(EntityEventArgs args)
        {
            var creep = args.Entity as Creep;

            if (creep == null || creep.ClassId != ClassId.CDOTA_BaseNPC_Creep_Lane || creep.Team == heroTeam)
            {
                return;
            }

            creepWaves.Select(x => x.Creeps).FirstOrDefault(x => x.Contains(creep))?.Remove(creep);
        }

        public void OnUpdate()
        {
            if (sleeper.Sleeping(this))
            {
                return;
            }

            if (sleeper.Sleeping("WavesAdded"))
            {
                sleeper.Sleep(300, this);
            }

            if (Game.IsPaused)
            {
                return;
            }

            var gameTime = Game.GameTime;

            if (gameTime < 0)
            {
                return;
            }

            var remove = creepWaves.Where(
                    wave => (wave.Creeps.Any() && wave.Creeps.All(x => !x.IsValid || !x.IsAlive))
                            || wave.CurrentPosition.Distance2D(wave.LaneData.Points.Last()) < 100
                            || (!wave.Creeps.Any() && Heroes.GetByTeam(heroTeam)
                                    .Any(x => wave.CurrentPosition.Distance2D(x) < 500)))
                .ToList();

            var canBeMerged = creepWaves.FirstOrDefault(
                x => creepWaves.Where(z => !z.Equals(x))
                    .Any(z => z.CurrentPosition.Distance2D(x.CurrentPosition) < 500));

            if (canBeMerged != null)
            {
                remove.Add(canBeMerged);
            }

            creepWaves.RemoveAll(x => remove.Contains(x));

            foreach (var creep in ObjectManager.GetEntities<Creep>()
                .Where(
                    x => x.IsSpawned && x.IsAlive && x.Team != heroTeam && !x.IsNeutral
                         && !creepWaves.SelectMany(z => z.Creeps).Contains(x)))
            {
                creepWaves.FirstOrDefault(x => x.CurrentPosition.Distance2D(creep) < 1000)?.Creeps.Add(creep);
            }

            if (sleeper.Sleeping("WavesAdded"))
            {
                return;
            }

            if (Math.Abs(gameTime % 30 - 1) < 0.5)
            {
                creepsData.LaneData.ForEach(x => creepWaves.Add(new CreepWave(x)));
                sleeper.Sleep(28000, "WavesAdded");
            }
        }

        private static bool IsPointOnLine(Vector3 point, Vector3 start, Vector3 end, float radius)
        {
            var endDistance = end.Distance2D(point);
            var startDistance = start.Distance2D(point);
            var distance = start.Distance2D(end);

            return Math.Abs(endDistance + startDistance - distance) < radius;
        }
    }
}
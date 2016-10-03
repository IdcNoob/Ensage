namespace PredictedCreepsLocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.Common.Objects;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Predictions
    {
        #region Fields

        private readonly List<CreepWave> creepWaves = new List<CreepWave>();

        private readonly MenuManager menuManager = new MenuManager();

        private DotaTexture creeepTexture;

        private CreepsData creepsData;

        private Hero hero;

        private Team heroTeam;

        private MultiSleeper sleeper;

        #endregion

        #region Public Methods and Operators

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

                    var middleCreep =
                        alive.OrderByDescending(x => x.Distance2D(wave.NextPoint)).ElementAtOrDefault(alive.Count / 2);

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

                    if (menuManager.ShowOnMapIcon)
                    {
                        Drawing.DrawRect(
                            position - new Vector2(0, mapSize),
                            new Vector2(mapSize, mapSize / 1.5f),
                            creeepTexture);
                    }
                    else
                    {
                        Drawing.DrawText(
                            "C",
                            "Arial",
                            position - new Vector2(0, mapSize),
                            new Vector2(mapSize),
                            Color.Orange,
                            FontFlags.None);
                    }
                }

                if (menuManager.ShowOnMinimapEnabled)
                {
                    var minimapSize = menuManager.ShowOnMinimapSize;

                    if (menuManager.ShowOnMinimapIcon)
                    {
                        var textureSize = new Vector2(minimapSize, minimapSize / 1.5f);
                        var position = WorldToMiniMap(wave.CurrentPosition, textureSize);
                        Drawing.DrawRect(position, textureSize, creeepTexture);
                    }
                    else
                    {
                        var position = WorldToMiniMap(wave.CurrentPosition, new Vector2(minimapSize));
                        Drawing.DrawText("C", "Arial", position, new Vector2(minimapSize), Color.Orange, FontFlags.None);
                    }
                }
            }
        }

        public void OnLoad()
        {
            hero = ObjectManager.LocalHero;
            heroTeam = hero.Team;
            creeepTexture =
                Drawing.GetTexture(
                    "materials/ensage_ui/heroes_horizontal/creep_" + (heroTeam == Team.Radiant ? "dire" : "radiant"));
            creepsData = new CreepsData(heroTeam);
            sleeper = new MultiSleeper();
        }

        public void OnRemoveEntity(EntityEventArgs args)
        {
            var creep = args.Entity as Creep;

            if (creep == null || creep.ClassID != ClassID.CDOTA_BaseNPC_Creep_Lane || creep.Team == heroTeam)
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

            var remove =
                creepWaves.Where(
                    wave =>
                    (wave.Creeps.Any() && wave.Creeps.All(x => !x.IsValid || !x.IsAlive))
                    || wave.CurrentPosition.Distance2D(wave.LaneData.Points.Last()) < 100
                    || (!wave.Creeps.Any()
                        && Heroes.GetByTeam(heroTeam).Any(x => wave.CurrentPosition.Distance2D(x) < 500))).ToList();

            var canBeMerged =
                creepWaves.FirstOrDefault(
                    x =>
                    creepWaves.Where(z => !z.Equals(x)).Any(z => z.CurrentPosition.Distance2D(x.CurrentPosition) < 500));

            if (canBeMerged != null)
            {
                remove.Add(canBeMerged);
            }

            creepWaves.RemoveAll(x => remove.Contains(x));

            foreach (var creep in
                ObjectManager.GetEntities<Creep>()
                    .Where(
                        x =>
                        x.IsSpawned && x.IsAlive && x.Team != heroTeam && !x.IsNeutral
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

        #endregion

        #region Methods

        private static bool IsPointOnLine(Vector3 point, Vector3 start, Vector3 end, float radius)
        {
            var endDistance = end.Distance2D(point);
            var startDistance = start.Distance2D(point);
            var distance = start.Distance2D(end);

            return Math.Abs(endDistance + startDistance - distance) < radius;
        }

        private static Vector2 WorldToMiniMap(Vector3 pos, Vector2 size)
        {
            const float MapLeft = -8000;
            const float MapTop = 7350;
            const float MapRight = 7500;
            const float MapBottom = -7200;
            var mapWidth = Math.Abs(MapLeft - MapRight);
            var mapHeight = Math.Abs(MapBottom - MapTop);

            var x = pos.X - MapLeft;
            var y = pos.Y - MapBottom;

            float dx, dy, px, py;
            if (Math.Round((float)Drawing.Width / Drawing.Height, 1) >= 1.7)
            {
                dx = 272f / 1920f * Drawing.Width;
                dy = 261f / 1080f * Drawing.Height;
                px = 11f / 1920f * Drawing.Width;
                py = 11f / 1080f * Drawing.Height;
            }
            else if (Math.Round((float)Drawing.Width / Drawing.Height, 1) >= 1.5)
            {
                dx = 267f / 1680f * Drawing.Width;
                dy = 252f / 1050f * Drawing.Height;
                px = 10f / 1680f * Drawing.Width;
                py = 11f / 1050f * Drawing.Height;
            }
            else
            {
                dx = 255f / 1280f * Drawing.Width;
                dy = 229f / 1024f * Drawing.Height;
                px = 6f / 1280f * Drawing.Width;
                py = 9f / 1024f * Drawing.Height;
            }
            var minimapMapScaleX = dx / mapWidth;
            var minimapMapScaleY = dy / mapHeight;

            var scaledX = Math.Min(Math.Max(x * minimapMapScaleX, 0), dx);
            var scaledY = Math.Min(Math.Max(y * minimapMapScaleY, 0), dy);

            var screenX = px + scaledX;
            var screenY = Drawing.Height - scaledY - py;

            return new Vector2((float)Math.Floor(screenX - size.X / 2), (float)Math.Floor(screenY - size.Y / 2));
        }

        #endregion
    }
}
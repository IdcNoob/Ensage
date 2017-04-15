namespace PredictedCreepsLocation
{
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Objects;

    using SharpDX;

    internal class CreepWave
    {
        private const int Speed = 325;

        private const int SpeedMax = 406;

        private const int SpeedMin = 243;

        public List<Creep> Creeps = new List<Creep>();

        private readonly float timeAdded;

        private bool speedUpdated;

        public CreepWave(LaneData laneData)
        {
            LaneData = laneData;
            PreviousPoint = laneData.Points.ElementAt(0);
            NextPoint = laneData.Points.ElementAt(1);
            timeAdded = Time = Game.GameTime;

            if (laneData.LanePosition == LanePosition.Middle)
            {
                speedUpdated = true;
            }
        }

        public Vector3 CurrentPosition { get; set; }

        public bool IgnorePoint { get; set; }

        public LaneData LaneData { get; set; }

        public Vector3 NextPoint { get; set; }

        public int PointIndex { get; set; } = 1;

        public Vector3 PreviousPoint { get; set; }

        public float Time { get; set; }

        public bool WasVisible { get; set; }

        public int GetSpeed()
        {
            if (!speedUpdated && Game.GameTime < 450)
            {
                var gameTime = Game.GameTime;
                switch (LaneData.LanePosition)
                {
                    case LanePosition.Top:
                        if (gameTime - timeAdded < 2)
                        {
                            return LaneData.Team == Team.Radiant ? SpeedMax : SpeedMin;
                        }
                        break;
                    case LanePosition.Bottom:
                        if (gameTime - timeAdded < 22 && LaneData.Team == Team.Radiant)
                        {
                            return SpeedMin;
                        }
                        if (gameTime - timeAdded < 10.5 && LaneData.Team == Team.Dire)
                        {
                            return SpeedMax;
                        }
                        break;
                }
                speedUpdated = true;
                IgnorePoint = true;
                Time = Game.GameTime;
                PreviousPoint = CurrentPosition;
            }
            return Speed;
        }

        public bool IsVisible(bool all = false)
        {
            return all
                       ? Creeps.Any() && Creeps.All(x => x.IsValid && x.IsVisible)
                       : Creeps.Any(x => x.IsValid && x.IsVisible);
        }
    }
}
namespace PredictedCreepsLocation.Creeps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.SDK.Extensions;

    using SharpDX;

    internal class CreepWave
    {
        private const int ChangedSpeedTime = 550;

        private const float SpeedDefault = 325;

        private const float SpeedMax = 422.5f;

        private const float SpeedMin = 211.25f;

        private readonly Dictionary<Team, float> maxSpeedTime = new Dictionary<Team, float>
        {
            { Team.Radiant, 3.8f },
            { Team.Dire, 1.7f },
        };

        private readonly Dictionary<Team, float> minSpeedTime = new Dictionary<Team, float>
        {
            { Team.Radiant, 1.5f },
            { Team.Dire, 5.1f },
        };

        private readonly int pathLength;

        private readonly Team team;

        private int lastPoint;

        private Vector3 lastVisiblePosition;

        private Vector3 predictedPosition;

        private Vector3[] remainingPath;

        private bool useChangedSpeed;

        public CreepWave(KeyValuePair<LanePosition, Vector3[]> laneData, Team waveTeam)
        {
            Lane = laneData.Key;
            Path = laneData.Value;
            pathLength = Path.Length;
            team = waveTeam;
        }

        public List<Creep> Creeps { get; } = new List<Creep>();

        public float Delay
        {
            get
            {
                if (useChangedSpeed)
                {
                    var timeSinceSpawn = Game.RawGameTime - SpawnTime;
                    switch (Lane)
                    {
                        case LanePosition.Easy:
                            if (timeSinceSpawn >= maxSpeedTime[team])
                            {
                                return ((SpeedDefault - SpeedMax) * Math.Max(
                                            maxSpeedTime[team] - Math.Max(LastVisibleTime - SpawnTime, 0),
                                            0)) / SpeedDefault;
                            }
                            break;
                        case LanePosition.Hard:
                            if (timeSinceSpawn >= minSpeedTime[team])
                            {
                                return ((SpeedDefault - SpeedMin) * Math.Max(
                                            minSpeedTime[team] - Math.Max(LastVisibleTime - SpawnTime, 0),
                                            0)) / SpeedDefault;
                            }
                            break;
                    }
                }

                return 0;
            }
        }

        public bool IsSpawned { get; private set; }

        public bool IsVisible
        {
            get
            {
                return Creeps.Any(x => x.IsValid && x.IsVisible);
            }
        }

        public LanePosition Lane { get; }

        public float LastVisibleTime { get; private set; }

        public Vector3[] Path { get; }

        public Vector3 Position
        {
            get
            {
                var creeps = Creeps.Where(x => x.IsValid && x.IsVisible).ToList();

                if (creeps.Count(x => x.IsVisible) <= creeps.Count / 2)
                {
                    return lastVisiblePosition;
                }

                lastVisiblePosition = creeps.Aggregate(new Vector3(), (vector3, creep) => vector3 + creep.Position) / creeps.Count;
                LastVisibleTime = Game.RawGameTime;
                remainingPath = null;

                return lastVisiblePosition;
            }
        }

        public Vector3 PredictedPosition
        {
            get
            {
                return predictedPosition;
            }
            set
            {
                predictedPosition = value;

                if (predictedPosition.Distance(Path[lastPoint]) < 500)
                {
                    lastPoint = Math.Min(lastPoint + 1, pathLength - 1);
                }
            }
        }

        public Vector3[] RemainingPath
        {
            get
            {
                if (remainingPath != null)
                {
                    return remainingPath;
                }

                var remainingPoints = pathLength - lastPoint;

                remainingPath = new Vector3[remainingPoints + 1];
                remainingPath[0] = lastVisiblePosition;
                Array.Copy(Path, lastPoint, remainingPath, 1, remainingPoints);

                return remainingPath;
            }
        }

        public float SpawnTime { get; private set; }

        public float Speed
        {
            get
            {
                if (useChangedSpeed)
                {
                    var timeSinceSpawn = Game.RawGameTime - SpawnTime;
                    switch (Lane)
                    {
                        case LanePosition.Easy:
                            if (timeSinceSpawn < maxSpeedTime[team])
                            {
                                return SpeedMax;
                            }
                            break;
                        case LanePosition.Hard:
                            if (timeSinceSpawn < minSpeedTime[team])
                            {
                                return SpeedMin;
                            }
                            break;
                    }
                }

                return SpeedDefault;
            }
        }

        public bool WasVisible
        {
            get
            {
                return Creeps.Any(x => x.IsValid && x.IsSpawned);
            }
        }

        public void Spawn()
        {
            IsSpawned = true;
            SpawnTime = Game.RawGameTime + Utils.GetLaneDelay(Lane, team);

            if (Lane != LanePosition.Middle && SpawnTime < ChangedSpeedTime)
            {
                useChangedSpeed = true;
            }
        }
    }
}
namespace Evader.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class Pathfinder : IDisposable
    {
        public enum EvadeMode
        {
            All,

            Disables,

            None
        }

        private const int StaticZ = 256;

        private readonly MultiSleeper sleeper = new MultiSleeper();

        private readonly Dictionary<Unit, uint> units = new Dictionary<Unit, uint>();

        public Pathfinder()
        {
            Pathfinding = new NavMeshPathfinding();
            Game.OnUpdate += OnUpdate;
        }

        public NavMeshPathfinding Pathfinding { get; }

        private static Hero Hero => Variables.Hero;

        public uint? AddObstacle(Vector3 position, float radius, uint? obstacle = null, int forceZ = StaticZ)
        {
            if (obstacle != null)
            {
                RemoveObstacle(obstacle.Value);
            }

            //todo delete setz after intersection fix
            return Pathfinding.AddObstacle(position.SetZ(forceZ), radius);
        }

        public uint? AddObstacle(
            Vector3 startPosition,
            Vector3 endPosition,
            float width,
            uint? obstacle = null,
            int forceZ = StaticZ)
        {
            if (obstacle != null)
            {
                RemoveObstacle(obstacle.Value);
            }

            //todo delete setz after intersection fix
            return Pathfinding.AddObstacle(startPosition.SetZ(forceZ), endPosition.SetZ(forceZ), width);
        }

        public uint? AddObstacle(
            Vector3 startPosition,
            Vector3 endPosition,
            float startWidth,
            float endWidth,
            uint? obstacle = null,
            int forceZ = StaticZ)
        {
            // todo: fix start => end widths
            // return Pathfinding.AddObstacle(startPosition, endPosition, startWidth, endWidth);

            if (obstacle != null)
            {
                RemoveObstacle(obstacle.Value);
            }

            //todo delete setz after intersection fix
            return Pathfinding.AddObstacle(startPosition.SetZ(forceZ), endPosition.SetZ(forceZ), endWidth);
        }

        public IEnumerable<Vector3> CalculateDebugPathFromObstacle(float remainingTime)
        {
            bool success;
            return Pathfinding.CalculatePathFromObstacle(
                Hero.NetworkPosition,
                Hero.NetworkPosition,
                Hero.RotationRad,
                Hero.MovementSpeed,
                (float)Hero.GetTurnRate(),
                remainingTime * 1000,
                false,
                out success);
        }

        public IEnumerable<Vector3> CalculateLongDebugPath(Vector3 endPosition)
        {
            bool success;
            return Pathfinding.CalculateLongPath(Hero.Position, endPosition, 5000, false, out success);
        }

        public IEnumerable<Vector3> CalculateLongPath(Vector3 endPosition, out bool success)
        {
            return Pathfinding.CalculateLongPath(Hero.Position, endPosition, 5000, true, out success);
        }

        public IEnumerable<Vector3> CalculateLongPath(Vector3 startPosition, Vector3 endPosition, out bool success)
        {
            return Pathfinding.CalculateLongPath(startPosition, endPosition, 5000, true, out success);
        }

        public IEnumerable<Vector3> CalculatePathFromObstacle(float remainingTime, out bool success)
        {
            return Pathfinding.CalculatePathFromObstacle(
                Hero.NetworkPosition,
                Hero.NetworkPosition,
                Hero.RotationRad,
                Hero.MovementSpeed,
                (float)Hero.GetTurnRate(),
                remainingTime * 1000,
                true,
                out success);
        }

        public IEnumerable<Vector3> CalculatePathFromObstacle(
            Vector3 startPosition,
            Vector3 endPosition,
            float remainingTime,
            out bool success)
        {
            return Pathfinding.CalculatePathFromObstacle(
                startPosition,
                endPosition,
                Hero.RotationRad,
                Hero.MovementSpeed,
                (float)Hero.GetTurnRate(),
                remainingTime * 1000,
                true,
                out success);
        }

        public IEnumerable<Vector3> CalculateStaticLongPath(Vector3 endPath, out bool success)
        {
            return Pathfinding.CalculateStaticLongPath(Hero.NetworkPosition, endPath, 5000, false, out success);
        }

        public void Dispose()
        {
            Game.OnUpdate -= OnUpdate;
        }

        public IEnumerable<uint> GetIntersectingObstacles(Hero hero)
        {
            //////////<<<<<<<<<<<
            var obstacles = Pathfinding.GetIntersectingObstacleIDs(hero.NetworkPosition, hero.HullRadius).ToList();
            if (hero.IsMoving || hero.IsTurning())
            {
                obstacles.AddRange(Pathfinding.GetIntersectingObstacleIDs(hero.InFront(150), hero.HullRadius));
            }

            return obstacles;
        }

        public IEnumerable<uint> GetIntersectingObstacles(Vector3 position, float radius)
        {
            return Pathfinding.GetIntersectingObstacleIDs(position, radius);
        }

        public void RemoveObstacle(uint id)
        {
            Pathfinding.RemoveObstacle(id);
        }

        public void UpdateObstacle(uint id, Vector3 position, float radius, int forceZ = StaticZ)
        {
            //todo delete setz after intersection fix
            Pathfinding.UpdateObstacle(id, position.SetZ(forceZ), radius);
        }

        public void UpdateObstacle(uint id, Vector3 position, float startWidth, float endWidth, int forceZ = StaticZ)
        {
            // FAIL
            // Pathfinding.UpdateObstacle(id, position, startWidth, endWidth);

            //todo delete setz after intersection fix
            Pathfinding.UpdateObstacle(id, position.SetZ(forceZ), endWidth, endWidth);
        }

        public void UpdateObstacle(uint id, Vector3 startPosition, Vector3 endPosition, int forceZ = StaticZ)
        {
            //todo delete setz after intersection fix
            Pathfinding.UpdateObstacle(id, startPosition.SetZ(forceZ), endPosition.SetZ(forceZ));
        }

        private void OnUpdate(EventArgs args)
        {
            if (!sleeper.Sleeping(units))
            {
                foreach (var unit in ObjectManager.GetEntities<Unit>()
                    .Where(x => x.IsValid && !units.ContainsKey(x) && x.IsSpawned && x.IsAlive && !x.Equals(Hero)))
                {
                    var obstacle = AddObstacle(unit.NetworkPosition, unit.HullRadius);
                    if (obstacle != null)
                    {
                        units.Add(unit, obstacle.Value);
                    }
                }

                sleeper.Sleep(1000, units);
            }

            if (!sleeper.Sleeping(Pathfinding))
            {
                var remove = new List<Unit>();
                foreach (var unitPair in units)
                {
                    var unit = unitPair.Key;
                    var obstacle = unitPair.Value;

                    if (unit == null || !unit.IsValid || !unit.IsAlive)
                    {
                        remove.Add(unit);
                        RemoveObstacle(obstacle);
                        continue;
                    }

                    UpdateObstacle(obstacle, unit.NetworkPosition, unit.HullRadius);
                }

                foreach (var unit in remove)
                {
                    units.Remove(unit);
                }

                sleeper.Sleep(100, Pathfinding);
            }
        }
    }
}
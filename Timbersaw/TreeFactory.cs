namespace Timbersaw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    using SharpDX;

    internal class TreeFactory
    {
        #region Fields

        private readonly List<Tree> allTrees = ObjectManager.GetEntities<Tree>().ToList();

        #endregion

        #region Public Methods and Operators

        public Vector3 GetBlinkPosition(Vector3 enemy, Vector3 hero, float distance, float radius)
        {
            var tree =
                allTrees.OrderBy(x => x.Distance2D(enemy))
                    .FirstOrDefault(
                        x =>
                        x.Distance2D(enemy) <= radius * 1.5 && x.Distance2D(hero) <= distance
                        && NavMesh.GetCellFlags(x.Position).HasFlag(NavMeshCellFlags.Tree));

            return tree != null
                       ? new Vector3((enemy.X + tree.Position.X) / 2, (enemy.Y + tree.Position.Y) / 2, enemy.Z)
                       : enemy;
        }

        public Tree GetChaseTree(Vector3 hero, Target target, float range, float maxDistanceToEnemy)
        {
            var targetPosition = target.GetPosition();
            var trees = GetAvailableTrees(hero, targetPosition, range).ToList();
            return
                trees.Where(
                    x =>
                    x.Distance2D(targetPosition) <= maxDistanceToEnemy
                    || Math.Abs(
                        target.Hero.FindAngleR() - Utils.DegreeToRadian(target.Hero.FindAngleForTurnTime(x.Position)))
                    < 1.5)
                    .FirstOrDefault(
                        z => trees.Where(x => !x.Equals(z)).All(x => !IsPointOnLine(z.Position, hero, x.Position, 25)));
        }

        public Tree GetDamageTree(Vector3 hero, Vector3 target, float range, float radius)
        {
            var trees = GetAvailableTrees(hero, target, range);
            return trees.FirstOrDefault(x => IsPointOnLine(x.Position, hero, target, radius));
        }

        #endregion

        #region Methods

        private static bool IsPointOnLine(Vector3 point, Vector3 start, Vector3 end, float radius)
        {
            var endDistance = end.Distance2D(point);
            var startDistance = start.Distance2D(point);
            var distance = start.Distance2D(end);

            return Math.Abs(distance + endDistance - startDistance) < (end.Distance2D(start) < radius ? radius : 50);
        }

        private IEnumerable<Tree> GetAvailableTrees(Vector3 hero, Vector3 target, float range)
        {
            return
                allTrees.OrderBy(x => x.Distance2D(target))
                    .Where(
                        x =>
                        x.Distance2D(hero) <= range && NavMesh.GetCellFlags(x.Position).HasFlag(NavMeshCellFlags.Tree));
        }

        #endregion
    }
}
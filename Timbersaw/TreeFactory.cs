namespace Timbersaw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using global::Timbersaw.Abilities;

    using SharpDX;

    internal class TreeFactory
    {
        #region Fields

        private readonly List<Tree> allTrees = ObjectManager.GetEntities<Tree>().ToList();

        #endregion

        #region Public Methods and Operators

        public bool CheckTree(Vector3 hero, Vector3 position, float range)
        {
            var distance = Math.Max(range, hero.Distance2D(position));
            var endPoint = hero.Extend(position, distance);

            return
                GetAvailableTrees(hero, endPoint, distance)
                    .Any(x => IsPointOnLine(x.Position, hero, endPoint, forceRadius: 25));
        }

        public Vector3 GetBlinkPosition(
            Target target,
            Vector3 hero,
            float distance,
            float radius,
            bool whirlingDeathCanBeCasted)
        {
            var tree =
                allTrees.OrderBy(x => x.Distance2D(target.Position))
                    .FirstOrDefault(
                        x =>
                        x.Distance2D(target.Position) <= radius * 1.9 && x.Distance2D(hero) <= distance
                        && NavMesh.GetCellFlags(x.Position).HasFlag(NavMeshCellFlags.Tree));

            return tree != null && whirlingDeathCanBeCasted
                       ? new Vector3(
                             (target.Position.X + tree.Position.X) / 2,
                             (target.Position.Y + tree.Position.Y) / 2,
                             target.Position.Z)
                       : target.GetPosition();
        }

        public Tree GetChaseTree(
            Vector3 hero,
            Target target,
            TimberChain timberChain,
            float maxDistanceToEnemy,
            float minDistanceToHero)
        {
            var castRange = timberChain.GetCastRange();
            var targetPosition = target.GetPosition();

            var targetDistance = target.GetDistance(hero);
            var ignoreMaxDistance = targetDistance > castRange + 200;

            var trees = GetAvailableTrees(hero, targetPosition, castRange).ToList();

            return
                trees.Where(
                    x =>
                    (ignoreMaxDistance
                     || x.Distance2D(
                         TimberPrediction.PredictedXYZ(
                             target,
                             timberChain.CastPoint + x.Distance2D(targetPosition) / timberChain.Speed))
                     <= maxDistanceToEnemy
                     || (Math.Abs(
                         target.Hero.FindAngleR() - Utils.DegreeToRadian(target.Hero.FindAngleForTurnTime(x.Position)))
                         < 0.3 && x.Distance2D(targetPosition) < targetDistance))
                    && x.Distance2D(hero) >= minDistanceToHero)
                    .FirstOrDefault(
                        z =>
                        trees.Where(x => !x.Equals(z))
                            .All(x => !IsPointOnLine(x.Position, hero, z.Position, forceRadius: 25)));
        }

        public Tree GetDamageTree(Vector3 hero, Vector3 target, float range, float radius)
        {
            var trees = GetAvailableTrees(hero, target, range);
            return trees.FirstOrDefault(x => IsPointOnLine(target, hero, x.Position, radius));
        }

        public Tree GetEscapeTree(Hero hero, Vector3 mouse, float range, float minRange)
        {
            var distance = Math.Min(range, hero.Distance2D(mouse));
            var trees = GetAvailableTrees(hero.Position, mouse, distance).ToList();

            return
                trees.FirstOrDefault(
                    z =>
                    Math.Abs(hero.FindAngleR() - Utils.DegreeToRadian(hero.FindAngleForTurnTime(z.Position))) < 0.3
                    && z.Distance2D(hero) > minRange
                    && trees.Where(x => !x.Equals(z))
                           .All(x => !IsPointOnLine(z.Position, hero.Position, x.Position, forceRadius: 25)));
        }

        #endregion

        #region Methods

        private static bool IsPointOnLine(
            Vector3 point,
            Vector3 start,
            Vector3 end,
            float dynamicRadius = 0,
            float forceRadius = 0)
        {
            var endDistance = end.Distance2D(point);
            var startDistance = start.Distance2D(point);
            var distance = start.Distance2D(end);

            return Math.Abs(endDistance + startDistance - distance)
                   < (forceRadius > 0 ? forceRadius : (end.Distance2D(start) < dynamicRadius ? dynamicRadius : 50));
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
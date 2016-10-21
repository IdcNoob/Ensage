namespace Timbersaw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abilities;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;
    using Ensage.Common.Objects.UtilityObjects;

    using SharpDX;

    internal class TreeFactory
    {
        #region Fields

        private readonly List<Tree> allTrees = ObjectManager.GetEntities<Tree>().ToList();

        private readonly Sleeper sleeper = new Sleeper();

        private readonly List<Tuple<Vector3, float, float>> unavailableTrees = new List<Tuple<Vector3, float, float>>();

        #endregion

        #region Public Methods and Operators

        public bool CheckTree(Hero hero, Vector3 position, TimberChain timberChain)
        {
            var distance = Math.Max(timberChain.GetCastRange(), hero.Distance2D(position));
            var endPoint = hero.Position.Extend(position, distance);
            var delay = Game.RawGameTime + timberChain.CastPoint + Game.Ping / 1000;

            return
                GetAvailableTrees(hero, endPoint, distance, delay, timberChain.Speed)
                    .Any(x => IsPointOnLine(x.Position, hero.Position, endPoint, 20));
        }

        public void ClearUnavailableTrees(bool complete = false)
        {
            if (complete)
            {
                unavailableTrees.Clear();
                return;
            }

            if (sleeper.Sleeping)
            {
                return;
            }

            unavailableTrees.RemoveAll(x => x.Item2 + 2 < Game.RawGameTime);
            sleeper.Sleep(2000);
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
            Hero hero,
            Target target,
            TimberChain timberChain,
            float maxDistanceToEnemy,
            float minDistanceToHero)
        {
            var castRange = timberChain.GetCastRange();
            var targetPosition = target.GetPosition();

            var targetDistance = target.GetDistance(hero.Position);
            var ignoreMaxDistance = targetDistance > castRange + 200;

            var delay = Game.RawGameTime + timberChain.CastPoint + Game.Ping / 1000;
            var trees = GetAvailableTrees(hero, targetPosition, castRange, delay, timberChain.Speed).ToList();

            return
                trees.Where(
                    x =>
                    (ignoreMaxDistance
                     || x.Distance2D(
                         TimberPrediction.PredictedXYZ(
                             target,
                             timberChain.CastPoint + x.Distance2D(targetPosition) / timberChain.Speed))
                     <= maxDistanceToEnemy
                     || (target.Hero.GetTurnTime(x.Position) <= 0 && x.Distance2D(targetPosition) < 600))
                    && x.Distance2D(hero) >= minDistanceToHero)
                    .FirstOrDefault(
                        z =>
                        trees.Where(x => !x.Equals(z))
                            .All(
                                x =>
                                x.Distance2D(hero) > 150 && !IsPointOnLine(x.Position, hero.Position, z.Position, 25)));
        }

        public Tree GetDamageTree(Hero hero, Vector3 target, TimberChain timberChain, bool dagger = false)
        {
            var delay = Game.RawGameTime + timberChain.CastPoint + Game.Ping / 1000;

            var trees = GetAvailableTrees(hero, target, timberChain.GetCastRange(), delay, timberChain.Speed).ToList();
            return
                trees.OrderBy(x => x.Distance2D(target))
                    .FirstOrDefault(
                        x =>
                        trees.Where(z => !z.Equals(x))
                            .All(
                                z =>
                                z.Distance2D(hero) > 150 && !IsPointOnLine(z.Position, hero.Position, x.Position, 25))
                        && (IsPointOnLine(target, hero.Position, x.Position, timberChain.Radius, false)
                            || x.Distance2D(target) < timberChain.Radius - 50) && (dagger || x.Distance2D(target) < 600));
        }

        public Tree GetMoveTree(Hero hero, Vector3 mouse, float range, float minRange)
        {
            var distance = Math.Min(range, hero.Distance2D(mouse));
            var trees = GetAvailableTrees(hero, mouse, distance).ToList();

            return
                trees.FirstOrDefault(
                    x =>
                    x.Distance2D(hero) > minRange
                    && x.Distance2D(mouse) + x.Distance2D(hero) < hero.Distance2D(mouse) + 100
                    && trees.Where(z => !z.Equals(x))
                           .All(
                               z =>
                               z.Distance2D(hero) > 150 && !IsPointOnLine(z.Position, hero.Position, x.Position, 25)));
        }

        public void SetUnavailableTrees(Vector3 start, Vector3 end, Chakram chakram)
        {
            var precision = chakram.Radius;
            var count = (int)Math.Ceiling(start.Distance2D(end) / precision);

            var ping = Game.Ping / 1000;
            var time = Game.RawGameTime;

            for (var i = 1; i <= count; i++)
            {
                var position = i == count ? end : start.Extend(end, precision * i);

                unavailableTrees.Add(
                    Tuple.Create(
                        position,
                        chakram.Radius,
                        chakram.CastPoint + ping + start.Distance2D(position) / chakram.Speed + time));
            }
        }

        public int TreesInPath(Hero hero, Vector3 position, float radius)
        {
            var trees = GetAvailableTrees(hero, position, hero.Distance2D(position));
            return trees.Count(x => IsPointOnLine(x.Position, hero.Position, position, radius));
        }

        #endregion

        #region Methods

        private static bool IsPointOnLine(
            Vector3 point,
            Vector3 start,
            Vector3 end,
            float radius,
            bool forceRadius = true)
        {
            var endDistance = end.Distance2D(point);
            var startDistance = start.Distance2D(point);
            var distance = start.Distance2D(end);

            return Math.Abs(endDistance + startDistance - distance)
                   < (forceRadius ? radius : (end.Distance2D(start) < radius ? radius : 50));
        }

        private IEnumerable<Tree> GetAvailableTrees(
            Unit hero,
            Vector3 target,
            float range,
            double time = 0,
            float speed = 1)
        {
            return
                allTrees.OrderBy(x => x.Distance2D(target))
                    .Where(
                        x =>
                        x.Distance2D(hero) <= range && NavMesh.GetCellFlags(x.Position).HasFlag(NavMeshCellFlags.Tree)
                        && (time <= 0
                            || !unavailableTrees.Any(
                                z =>
                                z.Item1.Distance2D(x) <= z.Item2
                                && time + hero.GetTurnTime(x) + x.Distance2D(hero) / speed >= z.Item3)));
        }

        #endregion
    }
}
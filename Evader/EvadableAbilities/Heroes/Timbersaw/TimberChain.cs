namespace Evader.EvadableAbilities.Heroes.Timbersaw
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using SharpDX;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class TimberChain : LinearProjectile
    {
        private readonly List<Tree> allTrees;

        private readonly float[] projectileSpeed = new float[4];

        public TimberChain(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsLowPureMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            allTrees = ObjectManager.GetEntities<Tree>().ToList();

            for (var i = 0u; i < projectileSpeed.Length; i++)
            {
                projectileSpeed[i] = Ability.AbilitySpecialData.First(x => x.Name == "speed").GetValue(i);
            }
        }

        public override void Check()
        {
            if (StartCast <= 0 && IsInPhase && AbilityOwner.IsVisible)
            {
                StartCast = Game.RawGameTime;
                EndCast = StartCast + CastPoint + GetCastRange() / GetProjectileSpeed();
            }
            else if (StartCast > 0 && Obstacle == null && CanBeStopped() && !AbilityOwner.IsTurning())
            {
                var tree = ChainTreePosition(AbilityOwner.InFront(GetCastRange()));

                if (tree == null)
                {
                    return;
                }

                StartPosition = AbilityOwner.NetworkPosition;
                EndPosition = tree.Position;
                AdditionalDelay = EndPosition.Distance2D(StartPosition) / GetProjectileSpeed();
                EndCast = StartCast + CastPoint + AdditionalDelay * 2;
                Obstacle = Pathfinder.AddObstacle(StartPosition, EndPosition, GetRadius(), GetEndRadius(), Obstacle);
            }
            else if (StartCast > 0 && Game.RawGameTime > EndCast)
            {
                End();
            }
            else if (Obstacle != null && !CanBeStopped())
            {
                Pathfinder.UpdateObstacle(Obstacle.Value, GetProjectilePosition(), GetRadius(), GetEndRadius());
            }
        }

        public override void End()
        {
            if (Obstacle != null)
            {
                Pathfinder.RemoveObstacle(Obstacle.Value);
            }

            AbilityDrawer.Dispose();
            Obstacle = null;
            StartCast = 0;
            EndCast = 0;
        }

        public override float GetProjectileSpeed()
        {
            return projectileSpeed[Ability.Level - 1];
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = hero.NetworkPosition;

            if (IsInPhase && position.Distance2D(StartPosition) <= GetRadius())
            {
                return StartCast + CastPoint + AdditionalDelay - Game.RawGameTime;
            }

            return StartCast + CastPoint + AdditionalDelay
                   + (position.Distance2D(StartPosition) - GetProjectileRadius(position) - 60) / GetProjectileSpeed()
                   - Game.RawGameTime;
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 100;
        }

        protected override Vector3 GetProjectilePosition(bool ignoreCastPoint = false)
        {
            return IsInPhase || Game.RawGameTime < StartCast + CastPoint + AdditionalDelay
                       ? StartPosition
                       : StartPosition.Extend(
                           EndPosition,
                           (Game.RawGameTime - StartCast - CastPoint - AdditionalDelay) * GetProjectileSpeed());
        }

        private Tree ChainTreePosition(Vector3 endPosition)
        {
            return allTrees
                .Where(
                    x => x.Distance2D(AbilityOwner) <= GetCastRange()
                         && NavMesh.GetCellFlags(x.Position).HasFlag(NavMeshCellFlags.Tree) && Math.Abs(
                             endPosition.Distance2D(x.Position) + AbilityOwner.Distance2D(x.Position)
                             - AbilityOwner.Distance2D(endPosition)) < 20)
                .OrderBy(x => AbilityOwner.Distance2D(x.Position))
                .ThenBy(x => AbilityOwner.FindRelativeAngle(x.Position))
                .FirstOrDefault();
        }
    }
}
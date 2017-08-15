namespace Evader.UsableAbilities.Abilities
{
    using System;
    using System.Linq;

    using Base;

    using Common;

    using Core;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using SharpDX;

    using AbilityType = Data.AbilityType;
    using LinearProjectile = EvadableAbilities.Base.LinearProjectile;
    using Projectile = EvadableAbilities.Base.Projectile;

    internal class BallLightning : Targetable
    {
        private Vector3 pointForLinearProjectile;

        public BallLightning(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            //todo improve
            CastPoint += 0.06f;
        }

        private static Pathfinder Pathfinder => Variables.Pathfinder;

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanCast();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            var projectile = ability as Projectile;

            if (projectile != null && !projectile.IsDisjointable)
            {
                Debugger.DrawGreenCircle(ability.AbilityOwner.NetworkPosition);

                return CastPoint + (float)Hero.GetTurnTime(ability.AbilityOwner);
            }

            var linearProjectile = ability as LinearProjectile;
            if (linearProjectile != null)
            {
                bool success;
                var pathfinderPoint = Pathfinder.CalculatePathFromObstacle(5, out success).LastOrDefault();

                var extendPoint = Hero.Position.Extend(
                    ability.AbilityOwner.Position,
                    linearProjectile.GetProjectileRadius(Hero.Position) + 50 * Ability.Level);

                var turnToPathfinder =
                    success ? CastPoint + (float)Hero.GetTurnTime(pathfinderPoint) * 1.35f : float.MaxValue;
                var turnToExtend = CastPoint + (float)Hero.GetTurnTime(extendPoint) * 1.35f;

                if (Math.Abs(turnToPathfinder - turnToExtend) < 0.075)
                {
                    if (Hero.Distance2D(pathfinderPoint) < Hero.Distance2D(extendPoint))
                    {
                        Debugger.DrawGreenCircle(pathfinderPoint);

                        pointForLinearProjectile = pathfinderPoint;
                        return turnToPathfinder;
                    }
                    else
                    {
                        Debugger.DrawGreenCircle(extendPoint);

                        pointForLinearProjectile = extendPoint;
                        return turnToExtend;
                    }
                }

                if (turnToPathfinder < turnToExtend)
                {
                    Debugger.DrawGreenCircle(pathfinderPoint);

                    pointForLinearProjectile = pathfinderPoint;
                    return turnToPathfinder;
                }
                else
                {
                    Debugger.DrawGreenCircle(extendPoint);

                    pointForLinearProjectile = extendPoint;
                    return turnToExtend;
                }
            }

            Debugger.DrawGreenCircle(Hero.NetworkPosition);

            return CastPoint;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            var projectile = ability as Projectile;

            if (projectile != null)
            {
                var pos = !projectile.IsDisjointable
                              ? Hero.NetworkPosition.Extend(projectile.StartPosition, 250)
                              : Hero.InFront(50);

                Debugger.DrawRedCircle(pos);

                Ability.UseAbility(pos, false, true);
            }
            else if (!pointForLinearProjectile.IsZero && pointForLinearProjectile.Distance2D(Hero) < 500)
            {
                Debugger.DrawRedCircle(pointForLinearProjectile);

                Ability.UseAbility(pointForLinearProjectile, false, true);
                pointForLinearProjectile = new Vector3();
            }
            else
            {
                var pos = Hero.InFront(150 + 20 * Ability.Level);

                Debugger.DrawRedCircle(pos);

                Ability.UseAbility(pos, false, true);
            }

            Sleep(CastPoint);
        }
    }
}
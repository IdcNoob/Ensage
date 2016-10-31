namespace Evader.UsableAbilities.Abilities
{
    using System;
    using System.Linq;

    using Base;

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
        #region Fields

        private Vector3 pointForLinearProjectile;

        #endregion

        #region Constructors and Destructors

        public BallLightning(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
            //todo improve
        }

        #endregion

        #region Properties

        private static Pathfinder Pathfinder => Variables.Pathfinder;

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            var projectile = ability as Projectile;

            if (projectile != null && !projectile.IsDisjointable)
            {
                return CastPoint + (float)Hero.GetTurnTime(ability.AbilityOwner);
            }

            var linearProjectile = ability as LinearProjectile;
            if (linearProjectile != null)
            {
                bool success;
                var pathfinderPoint = Pathfinder.CalculatePathFromObstacle(123, out success).LastOrDefault();

                var extendPoint = Hero.Position.Extend(
                    ability.AbilityOwner.Position,
                    linearProjectile.GetProjectileRadius(Hero.Position) + 50 * Ability.Level);

                var turnToPathfinder = success
                                           ? CastPoint + (float)Hero.GetTurnTime(pathfinderPoint) * 1.35f
                                           : float.MaxValue;
                var turnToExtend = CastPoint + (float)Hero.GetTurnTime(extendPoint) * 1.35f;

                if (Math.Abs(turnToPathfinder - turnToExtend) < 0.075)
                {
                    if (Hero.Distance2D(pathfinderPoint) < Hero.Distance2D(extendPoint))
                    {
                        pointForLinearProjectile = pathfinderPoint;
                        return turnToPathfinder;
                    }
                    else
                    {
                        pointForLinearProjectile = extendPoint;
                        return turnToExtend;
                    }
                }

                if (turnToPathfinder < turnToExtend)
                {
                    pointForLinearProjectile = pathfinderPoint;
                    return turnToPathfinder;
                }
                else
                {
                    pointForLinearProjectile = extendPoint;
                    return turnToExtend;
                }
            }

            return CastPoint;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            var projectile = ability as Projectile;

            if (projectile != null && !projectile.IsDisjointable)
            {
                Ability.UseAbility(
                    !projectile.IsDisjointable
                        ? Hero.NetworkPosition.Extend(ability.AbilityOwner.Position, 250)
                        : Hero.InFront(50));
            }
            else if (!pointForLinearProjectile.IsZero && pointForLinearProjectile.Distance2D(Hero) < 500)
            {
                Ability.UseAbility(pointForLinearProjectile);
                pointForLinearProjectile = new Vector3();
            }
            else
            {
                Ability.UseAbility(Hero.InFront(150 + 20 * Ability.Level));
            }

            Sleep(CastPoint);
        }

        #endregion
    }
}
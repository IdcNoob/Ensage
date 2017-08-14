namespace Evader.UsableAbilities.Base
{
    using System.Linq;

    using Common;

    using Core;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using SharpDX;

    using AbilityType = Data.AbilityType;

    internal class BlinkAbility : UsableAbility
    {
        public BlinkAbility(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        protected Vector3 BlinkPosition { get; set; }

        protected Pathfinder Pathfinder => Variables.Pathfinder;

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanUseItems() && !Hero.IsRuptured();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            var delay = CastPoint + Game.Ping / 1000;
            var requiredTime = (float)Hero.GetTurnTime(unit.Position) * 1.35f + delay + 0.1f;

            if (remainingTime - requiredTime > 0)
            {
                BlinkPosition = unit.Position;
                return requiredTime;
            }

            var left = remainingTime - delay;
            if (left < 0)
            {
                return 111;
            }

            BlinkPosition = Hero.GetBlinkPosition(unit.Position, left - 0.15f);

            return (float)Hero.GetTurnTime(BlinkPosition) + delay + 0.1f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            var range = GetCastRange() - 60;
            BlinkPosition = Hero.NetworkPosition.Extend(BlinkPosition, range);
            var obtsacles = Pathfinder.GetIntersectingObstacles(BlinkPosition, Hero.HullRadius);

            if (obtsacles.Any())
            {
                bool success;
                BlinkPosition = Pathfinder.CalculatePathFromObstacle(BlinkPosition, BlinkPosition, 5, out success)
                    .LastOrDefault();

                if (!success)
                {
                    //gg
                    return;
                }

                if (Hero.Distance2D(BlinkPosition) > range)
                {
                    // probably gg
                    BlinkPosition = Hero.NetworkPosition.Extend(BlinkPosition, range);
                }
            }

            Ability.UseAbility(BlinkPosition, false, true);
            Sleep();
        }
    }
}
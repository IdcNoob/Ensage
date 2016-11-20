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

    using AbilityType = Data.AbilityType;

    internal class BlinkAbility : UsableAbility
    {
        #region Constructors and Destructors

        public BlinkAbility(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Properties

        protected Pathfinder Pathfinder => Variables.Pathfinder;

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && !Hero.IsRuptured();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return CastPoint + (float)Hero.GetTurnTime(unit) * 1.25f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            var range = GetCastRange() - 60;
            var blinkPosition = Hero.NetworkPosition.Extend(target.Position, range);
            var obtsacles = Pathfinder.GetIntersectingObstacles(blinkPosition, Hero.HullRadius);

            if (obtsacles.Any())
            {
                bool success;
                blinkPosition =
                    Pathfinder.CalculatePathFromObstacle(blinkPosition, blinkPosition, 5, out success).LastOrDefault();

                if (!success)
                {
                    //gg
                    return;
                }

                if (Hero.Distance2D(blinkPosition) > range)
                {
                    // probably gg
                    blinkPosition = Hero.NetworkPosition.Extend(blinkPosition, range);
                }
            }

            Ability.UseAbility(blinkPosition);
            Sleep();
        }

        #endregion
    }
}
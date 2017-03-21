namespace Evader.UsableAbilities.Base
{
    using System.Linq;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class TargetTree : UsableAbility
    {
        #region Constructors and Destructors

        public TargetTree(Ability ability, AbilityType type, AbilityCastTarget target)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanCast()
                   && ((AOE)ability).StartPosition.Distance2D(Hero) <= Ability.GetCastRange();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return 0;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            var frontTree =
                ObjectManager.GetEntitiesFast<Tree>()
                    .Where(x => x.IsValid && x.IsAlive && x.Distance2D(Hero) < 350 && x.Name == "dota_temp_tree")
                    .OrderBy(x => target.FindRelativeAngle(x.Position))
                    .FirstOrDefault();

            if (frontTree == null)
            {
                return;
            }

            Ability.UseAbility(frontTree);
            Sleep();
        }

        #endregion
    }
}
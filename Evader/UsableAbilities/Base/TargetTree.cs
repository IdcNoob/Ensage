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
        public TargetTree(Ability ability, AbilityType type, AbilityCastTarget target)
            : base(ability, type, target)
        {
        }

        public override bool CanBeCasted(EvadableAbility ability, Unit unit)
        {
            return !Sleeper.Sleeping && Ability.CanBeCasted() && Hero.CanCast()
                   && ((AOE)ability).StartPosition.Distance2D(Hero) <= GetCastRange();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return 0;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            var frontTree = ObjectManager.GetEntitiesFast<Tree>()
                .Where(x => x.IsValid && x.IsAlive && x.Distance2D(Hero) < 300 && x.Name == "dota_temp_tree")
                .OrderBy(x => target.FindRelativeAngle(x.Position))
                .FirstOrDefault();

            if (frontTree == null)
            {
                return;
            }

            Ability.UseAbility(frontTree, false, true);
            Sleep();
        }

        protected override bool CheckEnemy(Unit unit)
        {
            return true;
        }

        protected override float GetCastRange()
        {
            return 400;
        }
    }
}
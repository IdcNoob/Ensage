namespace Evader.UsableAbilities.Items
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class ForceStaff : BlinkAbility
    {
        public ForceStaff(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.GetTurnTime(target) > 0)
            {
                Hero.Move(Hero.Position.Extend(target.Position, 40), false, true);
                Ability.UseAbility(Hero, true, true);
            }
            else
            {
                Ability.UseAbility(Hero, false, true);
            }

            Sleep();
        }
    }
}
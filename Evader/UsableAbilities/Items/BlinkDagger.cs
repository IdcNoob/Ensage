namespace Evader.UsableAbilities.Items
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using AbilityType = Data.AbilityType;

    internal class BlinkDagger : BlinkAbility
    {
        public BlinkDagger(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public void UseInFront()
        {
            Ability.UseAbility(Hero.InFront(1150), false, true);
            Sleep();
        }
    }
}
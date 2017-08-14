namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Extensions.SharpDX;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class Leap : BlinkAbility
    {
        public Leap(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return CastPoint + (float)Hero.GetTurnTime(unit) * 1.35f + 0.15f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.GetTurnTime(target) > 0)
            {
                Hero.Move(Hero.Position.Extend(target.Position, 40), false, true);
                Ability.UseAbility(true, false);
            }
            else
            {
                Ability.UseAbility(false, true);
            }

            Sleep();
        }
    }
}
namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class ElectricVortex : UsableAbility
    {
        public ElectricVortex(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            if (Hero.AghanimState())
            {
                return CastPoint;
            }
            return CastPoint + (unit.Equals(Hero) ? 0 : (float)Hero.GetTurnTime(unit) * 1.35f);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            if (Hero.AghanimState())
            {
                Ability.UseAbility(false, true);
            }
            else
            {
                Ability.UseAbility(target, false, true);
            }

            Sleep();
        }
    }
}
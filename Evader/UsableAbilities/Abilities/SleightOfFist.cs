namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class SleightOfFist : UsableAbility
    {
        //todo: improve
        public SleightOfFist(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit, float remainingTime)
        {
            return (float)Hero.GetTurnTime(unit) * 1.35f;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(target.NetworkPosition, false, true);
            Sleep();
        }
    }
}
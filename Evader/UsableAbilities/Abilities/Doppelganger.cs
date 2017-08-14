namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class Doppelganger : Targetable
    {
        public Doppelganger(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(target.BasePredict(target.MovementSpeed), false, true);
            Sleep();
        }
    }
}
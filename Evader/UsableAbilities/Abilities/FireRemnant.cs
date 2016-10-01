namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Core;

    using Ensage;
    using Ensage.Common.Extensions;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class FireRemnant : UsableAbility
    {
        #region Constructors and Destructors

        public FireRemnant(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool CanBeCasted(Unit unit)
        {
            return !Sleeper.Sleeping && Hero.HasModifier("modifier_ember_spirit_fire_remnant_timer")
                   && Ability.CanBeCasted() && Hero.CanCast();
        }

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return CastPoint;
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(Hero.InFront(250));
            Sleep();
        }

        #endregion
    }
}
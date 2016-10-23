namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class Snowball : Targetable
    {
        #region Constructors and Destructors

        public Snowball(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        //todo: take allies
        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(target);
            Sleep();
        }

        #endregion
    }
}
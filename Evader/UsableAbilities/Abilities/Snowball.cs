namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Core;

    using Ensage;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class Snowball : Targetable
    {
        #region Constructors and Destructors

        public Snowball(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
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
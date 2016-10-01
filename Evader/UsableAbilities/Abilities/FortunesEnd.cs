namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Core;

    using Ensage;

    using EvadableAbilities.Base;

    using AbilityType = Core.AbilityType;

    internal class FortunesEnd : Targetable
    {
        #region Constructors and Destructors

        public FortunesEnd(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(target);
            Hero.Stop(); // stop channeling
            Sleep();
        }

        #endregion
    }
}
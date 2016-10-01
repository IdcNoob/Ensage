namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Core;

    using Ensage;

    using EvadableAbilities.Base;

    using Utils;

    using AbilityType = Core.AbilityType;

    internal class SleightOfFist : UsableAbility
    {
        #region Constructors and Destructors

        //todo: improve
        public SleightOfFist(Ability ability, AbilityType type, AbilityFlags flags = AbilityFlags.None)
            : base(ability, type, flags)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override float GetRequiredTime(EvadableAbility ability, Unit unit)
        {
            return (float)Hero.GetTurnTime(unit);
        }

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(target.NetworkPosition);
            Sleep();
        }

        #endregion
    }
}
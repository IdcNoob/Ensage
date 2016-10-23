namespace Evader.UsableAbilities.Abilities
{
    using Base;

    using Data;

    using Ensage;
    using Ensage.Common;

    using EvadableAbilities.Base;

    using AbilityType = Data.AbilityType;

    internal class FortunesEnd : Targetable
    {
        #region Constructors and Destructors

        public FortunesEnd(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override void Use(EvadableAbility ability, Unit target)
        {
            Ability.UseAbility(target);
            DelayAction.Add(250 + Game.Ping, () => { Hero.Stop(); });
            Sleep();
        }

        #endregion
    }
}
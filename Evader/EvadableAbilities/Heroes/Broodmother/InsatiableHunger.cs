namespace Evader.EvadableAbilities.Heroes
{
    using Base;

    using Ensage;

    internal class InsatiableHunger : EvadableAbility
    {
        #region Constructors and Destructors

        public InsatiableHunger(Ability ability)
            : base(ability)
        {
            ModifierName = "modifier_broodmother_insatiable_hunger";
        }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
        }

        public override void Draw()
        {
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return 0;
        }

        #endregion
    }
}
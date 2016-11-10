namespace Evader.UsableAbilities.Items
{
    using Base;

    using Data;

    using Ensage;

    using AbilityType = Data.AbilityType;

    internal class BladeMail : NotTargetable
    {
        #region Constructors and Destructors

        public BladeMail(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        #endregion

        #region Public Methods and Operators

        public override bool UseCustomSleep(out float sleepTime)
        {
            sleepTime = 0;
            return true;
        }

        #endregion
    }
}
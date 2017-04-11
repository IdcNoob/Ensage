namespace Evader.UsableAbilities.Items
{
    using Base;

    using Data;

    using Ensage;

    using AbilityType = Data.AbilityType;

    internal class BladeMail : NotTargetable
    {
        public BladeMail(Ability ability, AbilityType type, AbilityCastTarget target = AbilityCastTarget.Self)
            : base(ability, type, target)
        {
        }

        public override bool UseCustomSleep(out float sleepTime)
        {
            sleepTime = 0;
            return true;
        }
    }
}
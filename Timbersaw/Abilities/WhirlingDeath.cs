namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common;
    using Ensage.Common.Extensions;

    internal class WhirlingDeath : TimberAbility
    {
        #region Constructors and Destructors

        public WhirlingDeath(Ability ability)
            : base(ability)
        {
            Radius = ability.GetCastRange() + 30;
        }

        #endregion

        #region Public Properties

        public float Radius { get; }

        #endregion

        #region Public Methods and Operators

        public void UseAbility(bool queue = false)
        {
            Ability.UseAbility(queue);
            Utils.Sleep(GetSleepTime + 500, "Timber." + Name);
        }

        #endregion
    }
}
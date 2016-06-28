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
            Radius = ability.GetRadius();
        }

        #endregion

        #region Public Properties

        public bool Combo { get; set; }

        public bool ComboDelayPassed => Utils.SleepCheck("Timber.Combo." + Name);

        public float Radius { get; }

        #endregion

        #region Public Methods and Operators

        public void SetComboDelay(double delay)
        {
            Utils.Sleep(delay, "Timber.Combo." + Name);
        }

        public void UseAbility(bool queue = false)
        {
            Ability.UseAbility(queue);
            Utils.Sleep(600, "Timber." + Name);
        }

        #endregion
    }
}
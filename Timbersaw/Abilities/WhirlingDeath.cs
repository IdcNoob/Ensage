namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    internal class WhirlingDeath : TimberAbility
    {
        #region Fields

        private readonly Sleeper comboSleeper;

        #endregion

        #region Constructors and Destructors

        public WhirlingDeath(Ability ability)
            : base(ability)
        {
            Radius = ability.GetRadius();
            comboSleeper = new Sleeper();
        }

        #endregion

        #region Public Properties

        public bool Combo { get; set; }

        public bool ComboDelayPassed => !comboSleeper.Sleeping;

        public float Radius { get; }

        #endregion

        #region Public Methods and Operators

        public void SetComboDelay(float delay)
        {
            comboSleeper.Sleep(delay);
        }

        public void UseAbility()
        {
            Ability.UseAbility();
            Sleeper.Sleep(1000);
        }

        #endregion
    }
}
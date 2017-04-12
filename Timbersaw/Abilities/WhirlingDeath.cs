namespace Timbersaw.Abilities
{
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Objects.UtilityObjects;

    internal class WhirlingDeath : TimberAbility
    {
        private readonly Sleeper comboSleeper;

        public WhirlingDeath(Ability ability)
            : base(ability)
        {
            Radius = ability.GetRadius();
            comboSleeper = new Sleeper();
        }

        public bool Combo { get; set; }

        public bool ComboDelayPassed => !comboSleeper.Sleeping;

        public float Radius { get; }

        public void SetComboDelay(float delay)
        {
            comboSleeper.Sleep(delay);
        }

        public void UseAbility()
        {
            if (!Ability.UseAbility())
            {
                return;
            }

            Sleeper.Sleep(1000);
        }
    }
}
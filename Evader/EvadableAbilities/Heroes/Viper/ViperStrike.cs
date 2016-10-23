namespace Evader.EvadableAbilities.Heroes.Viper
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class ViperStrike : Projectile
    {
        #region Constructors and Destructors

        public ViperStrike(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(TricksOfTheTrade);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
        }

        #endregion
    }
}
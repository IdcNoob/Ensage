namespace Evader.EvadableAbilities.Heroes.Oracle
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class FortunesEnd : Projectile
    {
        #region Constructors and Destructors

        public FortunesEnd(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
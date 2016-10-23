namespace Evader.EvadableAbilities.Heroes.PhantomAssassin
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class StiflingDagger : Projectile
    {
        #region Constructors and Destructors

        public StiflingDagger(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(Lotus);

            CounterAbilities.Remove(BladeMail);
        }

        #endregion
    }
}
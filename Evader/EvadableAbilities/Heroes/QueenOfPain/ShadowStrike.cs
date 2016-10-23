namespace Evader.EvadableAbilities.Heroes.QueenOfPain
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class ShadowStrike : Projectile
    {
        #region Constructors and Destructors

        public ShadowStrike(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.Add(Lotus);

            CounterAbilities.Remove("treant_living_armor");
        }

        #endregion
    }
}
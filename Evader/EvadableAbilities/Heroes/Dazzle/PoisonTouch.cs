namespace Evader.EvadableAbilities.Heroes.Dazzle
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class PoisonTouch : Projectile
    {
        #region Constructors and Destructors

        public PoisonTouch(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.BountyHunter
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class ShurikenToss : Projectile
    {
        #region Constructors and Destructors

        public ShurikenToss(Ability ability)
            : base(ability)
        {
            //todo: track check

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
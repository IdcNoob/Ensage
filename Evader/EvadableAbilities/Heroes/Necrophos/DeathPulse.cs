namespace Evader.EvadableAbilities.Heroes.Necrophos
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class DeathPulse : Projectile
    {
        #region Constructors and Destructors

        public DeathPulse(Ability ability)
            : base(ability)
        {
            IsDisjointable = false;

            // todo multi target ?
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        #endregion
    }
}
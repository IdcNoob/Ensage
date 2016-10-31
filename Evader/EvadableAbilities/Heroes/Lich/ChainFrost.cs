namespace Evader.EvadableAbilities.Heroes.Lich
{
    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class ChainFrost : Projectile
    {
        #region Constructors and Destructors

        public ChainFrost(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.AddRange(Invis);
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Items
{
    using Ensage;

    using static Core.Abilities;

    using Projectile = Base.Projectile;

    internal class EtherealBlade : Projectile
    {
        #region Constructors and Destructors

        public EtherealBlade(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Lotus);
        }

        #endregion
    }
}
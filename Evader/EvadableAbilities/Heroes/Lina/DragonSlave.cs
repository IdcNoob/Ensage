namespace Evader.EvadableAbilities.Heroes.Lina
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class DragonSlave : LinearProjectile
    {
        #region Constructors and Destructors

        public DragonSlave(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);
        }

        #endregion

        #region Public Methods and Operators

        public override float GetProjectileSpeed()
        {
            return base.GetProjectileSpeed() + 400;
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes
{
    using Ensage;

    using static Core.Abilities;

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
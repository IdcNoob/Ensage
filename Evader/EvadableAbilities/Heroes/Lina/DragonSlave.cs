namespace Evader.EvadableAbilities.Heroes.Lina
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class DragonSlave : LinearProjectile
    {
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

        public override float GetProjectileSpeed()
        {
            return base.GetProjectileSpeed() + 400;
        }
    }
}
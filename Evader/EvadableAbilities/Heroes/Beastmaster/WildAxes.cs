namespace Evader.EvadableAbilities.Heroes.Beastmaster
{
    using Ensage;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class WildAxes : LinearProjectile
    {
        private readonly float speed;

        public WildAxes(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(Armlet);
            CounterAbilities.Add(Bloodstone);

            speed = 800;
        }

        public override float GetProjectileSpeed()
        {
            return speed;
        }

        protected override float GetCastRange()
        {
            return base.GetCastRange() - 150;
        }
    }
}
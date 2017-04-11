namespace Evader.EvadableAbilities.Heroes.Jakiro
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class LiquidFire : Projectile, IModifier
    {
        private readonly float projectileSpeed;

        public LiquidFire(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);

            projectileSpeed = (float)AbilityOwner.ProjectileSpeed();
        }

        public EvadableModifier Modifier { get; }

        public override float GetProjectileSpeed()
        {
            return projectileSpeed;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            if (hero == null)
            {
                hero = Hero;
            }

            var position = ProjectileTarget?.NetworkPosition ?? hero.NetworkPosition;

            return position.Distance2D(GetProjectilePosition()) / GetProjectileSpeed();
        }
    }
}
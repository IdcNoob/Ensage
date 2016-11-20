namespace Evader.EvadableAbilities.Heroes.TemplarAssassin
{
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class Meld : Projectile, IModifier
    {
        #region Fields

        private readonly float projectileSpeed;

        #endregion

        #region Constructors and Destructors

        public Meld(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsPhys);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);

            projectileSpeed = (float)AbilityOwner.ProjectileSpeed();
        }

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion

        #region Public Methods and Operators

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

        #endregion
    }
}
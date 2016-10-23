namespace Evader.EvadableAbilities.Heroes.Alchemist
{
    using System.Linq;

    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class UnstableConcoction : Projectile, IModifier
    {
        #region Fields

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public UnstableConcoction(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(Invis);

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsPhys);

            IsDisjointable = false;
        }

        #endregion

        #region Public Properties

        public uint ModifierHandle { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void AddModifer(Modifier modifier, Hero hero)
        {
            if (hero.Team != HeroTeam)
            {
                return;
            }

            ModifierHandle = modifier.Handle;
            abilityModifier = modifier;
            modifierSource = hero;
        }

        public bool CanBeCountered()
        {
            return abilityModifier != null && abilityModifier.IsValid;
        }

        public override bool CanBeStopped()
        {
            return !Ability.IsHidden && base.CanBeStopped();
        }

        public float GetModiferRemainingTime()
        {
            return abilityModifier.RemainingTime;
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return allies.FirstOrDefault(x => x.Equals(modifierSource));
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
            modifierSource = null;
        }

        #endregion
    }
}
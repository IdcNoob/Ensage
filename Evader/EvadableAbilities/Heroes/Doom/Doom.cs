namespace Evader.EvadableAbilities.Heroes.Doom
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class Doom : LinearTarget, IModifier
    {
        #region Fields

        private readonly float modifierDuration;

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public Doom(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsMagic);

            modifierDuration = Ability.AbilitySpecialData.First(x => x.Name == "duration").Value;
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

            abilityModifier = modifier;
            modifierSource = hero;
            ModifierHandle = modifier.Handle;
        }

        public bool CanBeCountered()
        {
            return abilityModifier != null && abilityModifier.IsValid;
        }

        public float GetModiferRemainingTime()
        {
            return modifierDuration - abilityModifier.ElapsedTime;
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
namespace Evader.EvadableAbilities.Heroes.NightStalker
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class CripplingFear : LinearTarget, IModifier
    {
        #region Fields

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public CripplingFear(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);

            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.Add(Manta);
            ModifierAllyCounter.Add(Eul);
            ModifierAllyCounter.AddRange(AllyPurges);
            ModifierAllyCounter.AddRange(AllyShields);
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
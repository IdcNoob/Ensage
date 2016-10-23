namespace Evader.EvadableAbilities.Items
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class OrchidMalevolence : EvadableAbility, IModifier
    {
        #region Fields

        private readonly float modifierDuration;

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public OrchidMalevolence(Ability ability)
            : base(ability)
        {
            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.Add(Eul);
            ModifierAllyCounter.Add(Manta);
            ModifierAllyCounter.AddRange(AllyPurges);
            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(VsMagic);

            modifierDuration = Ability.AbilitySpecialData.First(x => x.Name == "silence_duration").Value;
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

        public override void Check()
        {
        }

        public override void Draw()
        {
        }

        public float GetModiferRemainingTime()
        {
            return modifierDuration - abilityModifier.ElapsedTime;
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return allies.FirstOrDefault(x => x.Equals(modifierSource));
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return 0;
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
            modifierSource = null;
        }

        #endregion
    }
}
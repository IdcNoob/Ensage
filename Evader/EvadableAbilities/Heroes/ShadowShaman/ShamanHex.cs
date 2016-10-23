namespace Evader.EvadableAbilities.Heroes.ShadowShaman
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class ShamanHex : EvadableAbility, IModifier
    {
        #region Fields

        private readonly float[] modifierDuration = new float[4];

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public ShamanHex(Ability ability)
            : base(ability)
        {
            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.AddRange(AllyPurges);
            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsMagic);

            for (var i = 0u; i < 4; i++)
            {
                modifierDuration[i] = Ability.AbilitySpecialData.First(x => x.Name == "duration").GetValue(i);
            }
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

        public override void Check()
        {
        }

        public override void Draw()
        {
        }

        public float GetModiferRemainingTime()
        {
            return modifierDuration[Ability.Level - 1] - abilityModifier.ElapsedTime;
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
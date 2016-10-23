namespace Evader.EvadableAbilities.Heroes.SkywrathMage
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class AncientSeal : EvadableAbility, IModifier
    {
        #region Fields

        private readonly float[] modifierDuration = new float[4];

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public AncientSeal(Ability ability)
            : base(ability)
        {
            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.Add(Eul);
            ModifierAllyCounter.Add(Manta);
            ModifierAllyCounter.AddRange(AllyPurges);
            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(VsMagic);

            for (var i = 0u; i < 4; i++)
            {
                modifierDuration[i] = Ability.AbilitySpecialData.First(x => x.Name == "seal_duration").GetValue(i);
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
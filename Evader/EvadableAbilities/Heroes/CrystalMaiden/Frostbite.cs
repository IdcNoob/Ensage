namespace Evader.EvadableAbilities.Heroes.CrystalMaiden
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class Frostbite : LinearTarget, IModifier
    {
        #region Fields

        private readonly float[] modifierDuration = new float[4];

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public Frostbite(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.Add(FortunesEnd);
            ModifierAllyCounter.Add(PhaseShift);
            ModifierAllyCounter.Add(Eul);
            ModifierAllyCounter.Add(Doppelganger);
            ModifierAllyCounter.Add(Manta);
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
            if (hero.Team != HeroTeam || modifier.Name == "modifier_stunned")
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
            return modifierDuration[Ability.Level - 1] - abilityModifier.ElapsedTime;
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return allies.FirstOrDefault(x => x.Equals(modifierSource));
        }

        public void RemoveModifier(Modifier modifier)
        {
            if (modifier.Name == "modifier_stunned")
            {
                return;
            }

            abilityModifier = null;
            modifierSource = null;
        }

        #endregion
    }
}
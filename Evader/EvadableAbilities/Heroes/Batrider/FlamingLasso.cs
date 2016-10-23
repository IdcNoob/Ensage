namespace Evader.EvadableAbilities.Heroes.Batrider
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class FlamingLasso : LinearTarget, IModifier
    {
        #region Fields

        private readonly float[] modifierDuration = new float[3];

        private readonly string modifierName;

        private Modifier abilityModifier;

        #endregion

        #region Constructors and Destructors

        public FlamingLasso(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);
            CounterAbilities.AddRange(Invul);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);

            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.Add(FalsePromise);
            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(VsMagic);

            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(Invul);

            for (var i = 0u; i < 3; i++)
            {
                modifierDuration[i] = Ability.AbilitySpecialData.First(x => x.Name == "duration").GetValue(i);
            }

            modifierName = "modifier_batrider_flaming_lasso";
        }

        #endregion

        #region Public Properties

        public uint ModifierHandle { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void AddModifer(Modifier modifier, Hero hero)
        {
            if (hero.Team == HeroTeam)
            {
                return;
            }

            abilityModifier = modifier;
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
            return allies.OrderBy(x => x.Health).FirstOrDefault(x => x.HasModifier(modifierName));
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion
    }
}
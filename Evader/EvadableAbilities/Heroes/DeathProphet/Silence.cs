namespace Evader.EvadableAbilities.Heroes.DeathProphet
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class Silence : LinearAOE, IModifier
    {
        #region Fields

        private readonly float[] modifierDuration = new float[4];

        private Modifier abilityModifier;

        #endregion

        #region Constructors and Destructors

        public Silence(Ability ability)
            : base(ability)
        {
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsMagic);

            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.Add(Eul);
            ModifierAllyCounter.Add(Manta);
            ModifierAllyCounter.AddRange(AllyPurges);
            ModifierAllyCounter.AddRange(AllyShields);
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
            return
                allies.Where(x => x.HasModifier(abilityModifier.Name))
                    .OrderByDescending(x => x.Equals(Hero))
                    .ThenBy(x => x.Health)
                    .FirstOrDefault();
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion
    }
}
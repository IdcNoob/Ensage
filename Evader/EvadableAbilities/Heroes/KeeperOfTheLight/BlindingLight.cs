namespace Evader.EvadableAbilities.Heroes.KeeperOfTheLight
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class BlindingLight : LinearAOE, IModifier
    {
        #region Fields

        private readonly float[] modifierDuration = new float[3];

        private Modifier abilityModifier;

        #endregion

        #region Constructors and Destructors

        public BlindingLight(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.AddRange(VsLowDisable);

            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.Add(FortunesEnd);
            ModifierAllyCounter.Add(Manta);
            ModifierAllyCounter.AddRange(AllyPurges);

            for (var i = 0u; i < 3; i++)
            {
                modifierDuration[i] = Ability.AbilitySpecialData.First(x => x.Name == "miss_duration").GetValue(i);
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
                allies.Where(x => x.HasModifier(abilityModifier.Name)).OrderByDescending(x => x.Health).FirstOrDefault();
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion
    }
}
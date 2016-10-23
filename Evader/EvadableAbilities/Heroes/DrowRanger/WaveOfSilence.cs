namespace Evader.EvadableAbilities.Heroes.DrowRanger
{
    using System.Linq;

    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    using LinearProjectile = Base.LinearProjectile;

    internal class WaveOfSilence : LinearProjectile, IModifier
    {
        #region Fields

        private readonly float[] modifierDuration = new float[4];

        private Modifier abilityModifier;

        #endregion

        #region Constructors and Destructors

        public WaveOfSilence(Ability ability)
            : base(ability)
        {
            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);

            ModifierAllyCounter.Add(Lotus);
            ModifierAllyCounter.Add(FortunesEnd);
            ModifierAllyCounter.Add(Eul);
            ModifierAllyCounter.Add(Manta);
            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsMagic);

            for (var i = 0u; i < 4; i++)
            {
                modifierDuration[i] = Ability.AbilitySpecialData.First(x => x.Name == "silence_duration").GetValue(i);
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

        public override float GetProjectileSpeed()
        {
            return base.GetProjectileSpeed() + 200;
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion

        #region Methods

        protected override float GetCastRange()
        {
            return base.GetCastRange() + 100;
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.Sven
{
    using System.Linq;

    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    using Projectile = Base.Projectile;

    internal class StormHammer : Projectile, IModifier
    {
        #region Fields

        private readonly float modifierDuration;

        private Modifier abilityModifier;

        #endregion

        #region Constructors and Destructors

        public StormHammer(Ability ability)
            : base(ability)
        {
            //todo add projectile aoe stun

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(BallLightning);
            CounterAbilities.Add(SleightOfFist);
            CounterAbilities.Add(Manta);
            CounterAbilities.Add(Eul);
            CounterAbilities.AddRange(VsDisable);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(SnowBall);
            CounterAbilities.Add(Lotus);
            CounterAbilities.Remove("abaddon_aphotic_shield");

            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.AddRange(VsMagic);

            modifierDuration = Ability.AbilitySpecialData.First(x => x.Name == "bolt_stun_duration").Value;
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
            return modifierDuration - abilityModifier.ElapsedTime;
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
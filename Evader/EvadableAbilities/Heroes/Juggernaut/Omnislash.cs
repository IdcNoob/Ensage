namespace Evader.EvadableAbilities.Heroes.Juggernaut
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class Omnislash : LinearTarget, IModifier
    {
        #region Fields

        private readonly float[] modifierDuration = new float[3];

        private readonly float modifierRadius;

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public Omnislash(Ability ability)
            : base(ability)
        {
            BlinkAbilities.AddRange(BlinkAbilityNames);
            DisableAbilities.AddRange(DisableAbilityNames);

            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(VsDamage);
            CounterAbilities.AddRange(VsPhys);
            CounterAbilities.AddRange(Invis);
            CounterAbilities.Add(Bloodstone);
            CounterAbilities.Remove("item_blade_mail");

            ModifierAllyCounter.Add(PhaseShift);
            ModifierAllyCounter.Add(Eul);
            ModifierAllyCounter.AddRange(Invul);
            ModifierAllyCounter.Add(Manta);
            ModifierAllyCounter.AddRange(VsDamage);
            ModifierAllyCounter.AddRange(VsPhys);
            ModifierAllyCounter.AddRange(AllyShields);

            modifierRadius = Ability.AbilitySpecialData.First(x => x.Name == "omni_slash_radius").Value;
            var interval = Ability.AbilitySpecialData.First(x => x.Name == "omni_slash_bounce_tick").Value;
            for (var i = 0u; i < 3; i++)
            {
                modifierDuration[i] = interval
                                      * (Ability.AbilitySpecialData.First(x => x.Name == "omni_slash_jumps").GetValue(i)
                                         - 1);
            }
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
            return
                allies.Where(x => x.Distance2D(modifierSource) <= modifierRadius)
                    .OrderByDescending(x => x.Equals(Hero))
                    .ThenBy(x => x.Health)
                    .FirstOrDefault();
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
            modifierSource = null;
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.AncientApparition
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class IceBlast : AOE, IModifier

    {
        #region Fields

        private Modifier abilityModifier;

        #endregion

        #region Constructors and Destructors

        public IceBlast(Ability ability)
            : base(ability)
        {
            ModifierAllyCounter.AddRange(AllyShields);
            ModifierAllyCounter.AddRange(VsMagic);

            ModifierAllyCounter.Remove("legion_commander_press_the_attack");
            ModifierAllyCounter.Remove("treant_living_armor");
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

        public override void Check()
        {
            //todo fix
        }

        public float GetModiferRemainingTime()
        {
            return abilityModifier.RemainingTime;
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return
                allies.OrderByDescending(x => x.Equals(Hero))
                    .ThenBy(x => x.Health / x.MaximumHealth)
                    .FirstOrDefault(x => x.HasModifier(abilityModifier.Name));
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.Abaddon
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;

    using static Data.AbilityNames;

    internal class BorrowedTime : EvadableAbility, IModifier
    {
        #region Fields

        private Modifier abilityModifier;

        #endregion

        #region Constructors and Destructors

        public BorrowedTime(Ability ability)
            : base(ability)
        {
            ModifierEnemyCounter.Add(Eul);
            ModifierEnemyCounter.AddRange(Invul);
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

            ModifierHandle = modifier.Handle;
            abilityModifier = modifier;
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
            return abilityModifier.RemainingTime;
        }

        public Hero GetModifierHero(ParallelQuery<Hero> allies)
        {
            return null;
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return 0;
        }

        public void RemoveModifier(Modifier modifier)
        {
            abilityModifier = null;
        }

        #endregion
    }
}
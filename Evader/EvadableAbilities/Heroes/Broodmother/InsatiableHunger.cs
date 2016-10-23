namespace Evader.EvadableAbilities.Heroes.Broodmother
{
    using System.Linq;

    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using static Data.AbilityNames;

    internal class InsatiableHunger : EvadableAbility, IModifier
    {
        #region Fields

        private readonly float modifierRadius;

        private Modifier abilityModifier;

        private Hero modifierSource;

        #endregion

        #region Constructors and Destructors

        public InsatiableHunger(Ability ability)
            : base(ability)
        {
            ModifierAllyCounter.AddRange(VsDamage);
            ModifierAllyCounter.AddRange(VsPhys);

            ModifierEnemyCounter.Add(Eul);
            ModifierEnemyCounter.Add(FortunesEnd);
            ModifierEnemyCounter.Add(Decrepify);

            modifierRadius = 200;
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
            return
                allies.Where(x => x.Distance2D(modifierSource) <= modifierRadius)
                    .OrderBy(x => AbilityOwner.FindRelativeAngle(x.Position))
                    .FirstOrDefault();
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
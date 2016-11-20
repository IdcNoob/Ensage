namespace Evader.EvadableAbilities.Heroes.SpiritBreaker
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;
    using Modifiers.Special;

    using static Data.AbilityNames;

    internal class ChargeOfDarkness : EvadableAbility, IModifier
    {
        #region Constructors and Destructors

        public ChargeOfDarkness(Ability ability)
            : base(ability)
        {
            Modifier = new ChargeOfDarknessModifier(
                HeroTeam,
                EvadableModifier.GetHeroType.ModifierSource,
                AbilityOwner,
                ignoreRemainingTime: true);

            Modifier.AllyCounterAbilities.Add(PhaseShift);
            Modifier.AllyCounterAbilities.Add(BallLightning);
            Modifier.AllyCounterAbilities.Add(Eul);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.AddRange(VsDisable);
            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(Invis);
            Modifier.AllyCounterAbilities.Add(SnowBall);
            Modifier.AllyCounterAbilities.Add(Lotus);

            Modifier.EnemyCounterAbilities.AddRange(DisableAbilityNames);
        }

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion

        #region Public Methods and Operators

        public override void Check()
        {
        }

        public override void Draw()
        {
        }

        public override float GetRemainingTime(Hero hero = null)
        {
            return 0;
        }

        #endregion
    }
}
namespace Evader.EvadableAbilities.Heroes.Pugna
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class LifeDrain : LinearTarget, IModifier
    {
        #region Constructors and Destructors

        public LifeDrain(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                HeroTeam,
                EvadableModifier.GetHeroType.ModifierSource,
                ignoreRemainingTime: true);

            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(VsMagic);

            Modifier.AllyCounterAbilities.Add(PhaseShift);
            Modifier.AllyCounterAbilities.Add(Eul);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.Add(SleightOfFist);
            Modifier.AllyCounterAbilities.Add(BallLightning);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
        }

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion
    }
}
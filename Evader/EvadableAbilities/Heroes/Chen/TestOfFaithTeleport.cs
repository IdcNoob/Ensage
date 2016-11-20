namespace Evader.EvadableAbilities.Heroes.Chen
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class TestOfFaithTeleport : NoObstacleAbility, IModifier
    {
        #region Constructors and Destructors

        public TestOfFaithTeleport(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(EnemyTeam, EvadableModifier.GetHeroType.ModifierSource, 1);

            DisableAbilities.AddRange(DisableAbilityNames);
            DisableAbilities.Remove(Eul);

            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.AddRange(Diffusal);
        }

        #endregion

        #region Public Properties

        public EvadableModifier Modifier { get; }

        #endregion
    }
}
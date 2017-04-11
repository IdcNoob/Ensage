namespace Evader.EvadableAbilities.Heroes.Chen
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class TestOfFaithTeleport : NoObstacleAbility, IModifier
    {
        public TestOfFaithTeleport(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(EnemyTeam, EvadableModifier.GetHeroType.ModifierSource, 1);

            DisableAbilities.AddRange(DisableAbilityNames);
            DisableAbilities.Remove(Eul);

            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.AddRange(Diffusal);
        }

        public EvadableModifier Modifier { get; }
    }
}
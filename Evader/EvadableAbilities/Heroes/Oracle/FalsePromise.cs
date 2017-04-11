namespace Evader.EvadableAbilities.Heroes.Oracle
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class FalsePromise : NoObstacleAbility, IModifier
    {
        public FalsePromise(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(EnemyTeam, EvadableModifier.GetHeroType.ModifierSource);

            DisableAbilities.AddRange(DisableAbilityNames);

            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.AddRange(Invul);
            Modifier.EnemyCounterAbilities.AddRange(DisableAbilityNames);
        }

        public EvadableModifier Modifier { get; }
    }
}
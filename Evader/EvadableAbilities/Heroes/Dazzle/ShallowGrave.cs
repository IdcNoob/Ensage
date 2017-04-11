namespace Evader.EvadableAbilities.Heroes.Dazzle
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class ShallowGrave : NoObstacleAbility, IModifier
    {
        public ShallowGrave(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(EnemyTeam, EvadableModifier.GetHeroType.ModifierSource);

            DisableAbilities.AddRange(DisableAbilityNames);

            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.AddRange(Invul);
        }

        public EvadableModifier Modifier { get; }
    }
}
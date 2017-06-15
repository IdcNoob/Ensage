namespace Evader.EvadableAbilities.Heroes.Razor
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class StaticLink : LinearTarget, IModifier
    {
        public StaticLink(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: AbilityOwner.AttackRange + 150);

            CounterAbilities.Add(Lotus);
            CounterAbilities.AddRange(Invis);

            Modifier.AllyCounterAbilities.AddRange(AllyShields);

            Modifier.EnemyCounterAbilities.Add(HurricanePike);
            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.Add(FortunesEnd);
            Modifier.EnemyCounterAbilities.Add(Decrepify);
            Modifier.EnemyCounterAbilities.AddRange(Invul);
        }

        public EvadableModifier Modifier { get; }
    }
}
namespace Evader.EvadableAbilities.Heroes.EmberSpirit
{
    using Base;
    using Base.Interfaces;

    using Ensage;
    using Ensage.Common.Extensions;

    using Modifiers;

    using static Data.AbilityNames;

    internal class FlameGuard : EvadableAbility, IModifier
    {
        public FlameGuard(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: Ability.GetRadius());

            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.Add(FortunesEnd);

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
        }

        public EvadableModifier Modifier { get; }

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
    }
}
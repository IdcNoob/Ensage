namespace Evader.EvadableAbilities.Heroes.Broodmother
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class InsatiableHunger : EvadableAbility, IModifier
    {
        public InsatiableHunger(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: AbilityOwner.AttackRange + 150);

            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);

            Modifier.EnemyCounterAbilities.Add(Decrepify);
            Modifier.EnemyCounterAbilities.AddRange(Invul);
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
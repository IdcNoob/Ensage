namespace Evader.EvadableAbilities.Heroes.PhantomAssassin
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class PhantomStrike : EvadableAbility, IModifier
    {
        public PhantomStrike(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: AbilityOwner.AttackRange + 150);

            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);

            Modifier.EnemyCounterAbilities.Add(Decrepify);
            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.AddRange(VsPhys);
            Modifier.EnemyCounterAbilities.Add(FortunesEnd);
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
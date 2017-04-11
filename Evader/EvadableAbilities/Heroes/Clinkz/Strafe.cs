namespace Evader.EvadableAbilities.Heroes.Clinkz
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Strafe : EvadableAbility, IModifier
    {
        public Strafe(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                EnemyTeam,
                EvadableModifier.GetHeroType.ClosestToSource,
                maxDistanceToSource: AbilityOwner.AttackRange + 150);

            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsPhys);

            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.Add(FortunesEnd);
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
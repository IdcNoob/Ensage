namespace Evader.EvadableAbilities.Heroes.Huskar
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class InnerVitality : EvadableAbility, IModifier
    {
        public InnerVitality(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(EnemyTeam, EvadableModifier.GetHeroType.ModifierSource);

            Modifier.EnemyCounterAbilities.Add(Eul);
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
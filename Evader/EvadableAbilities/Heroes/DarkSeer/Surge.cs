namespace Evader.EvadableAbilities.Heroes.DarkSeer
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Surge : EvadableAbility, IModifier
    {
        public Surge(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(EnemyTeam, EvadableModifier.GetHeroType.ModifierSource);

            Modifier.EnemyCounterAbilities.Add(FortunesEnd);
            Modifier.EnemyCounterAbilities.Add(Eul);
            Modifier.EnemyCounterAbilities.AddRange(Invul);
            Modifier.EnemyCounterAbilities.AddRange(DisableAbilityNames);
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
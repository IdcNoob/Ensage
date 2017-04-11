namespace Evader.EvadableAbilities.Heroes.Disruptor
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class ThunderStrike : EvadableAbility, IModifier
    {
        public ThunderStrike(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.ModifierSource);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
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
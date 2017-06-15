namespace Evader.EvadableAbilities.Heroes.SkywrathMage
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class MysticFlare : EvadableAbility, IModifier
    {
        public MysticFlare(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                HeroTeam,
                EvadableModifier.GetHeroType.LowestHealth,
                ignoreRemainingTime: true);

            Modifier.AllyCounterAbilities.Add(HurricanePike);
            Modifier.AllyCounterAbilities.Add(PhaseShift);
            Modifier.AllyCounterAbilities.Add(Eul);
            Modifier.AllyCounterAbilities.Add(TricksOfTheTrade);
            Modifier.AllyCounterAbilities.AddRange(VsDamage);
            Modifier.AllyCounterAbilities.AddRange(VsMagic);
            Modifier.AllyCounterAbilities.AddRange(Invul);
            Modifier.AllyCounterAbilities.Add(SnowBall);
            Modifier.AllyCounterAbilities.Add(Armlet);
            Modifier.AllyCounterAbilities.AddRange(Invis);
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
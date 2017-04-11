namespace Evader.EvadableAbilities.Heroes.Pudge
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Rot : EvadableAbility, IModifier
    {
        public Rot(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                HeroTeam,
                EvadableModifier.GetHeroType.LowestHealth,
                ignoreRemainingTime: true);

            DisableTimeSinceCastCheck = true;

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
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
namespace Evader.EvadableAbilities.Heroes.Sniper
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Shrapnel : EvadableAbility, IModifier
    {
        public Shrapnel(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(
                HeroTeam,
                EvadableModifier.GetHeroType.LowestHealth,
                ignoreRemainingTime: true);

            DisableTimeSinceCastCheck = true;

            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.Remove("legion_commander_press_the_attack");
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
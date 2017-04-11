namespace Evader.EvadableAbilities.Heroes.Earthshaker
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class Aftershock : EvadableAbility, IModifier
    {
        public Aftershock(Ability ability)
            : base(ability)
        {
            DisableTimeSinceCastCheck = true; // just for fix

            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            Modifier.AllyCounterAbilities.Add(Enrage);
            Modifier.AllyCounterAbilities.AddRange(AllyShields);
            Modifier.AllyCounterAbilities.AddRange(Invul);
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
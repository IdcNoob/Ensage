namespace Evader.EvadableAbilities.Heroes.ShadowDemon
{
    using Base;
    using Base.Interfaces;

    using Ensage;

    using Modifiers;

    using static Data.AbilityNames;

    internal class SoulCatcher : LinearAOE, IModifier
    {
        public SoulCatcher(Ability ability)
            : base(ability)
        {
            Modifier = new EvadableModifier(HeroTeam, EvadableModifier.GetHeroType.LowestHealth);

            CounterAbilities.Add(PhaseShift);
            CounterAbilities.Add(SleightOfFist);

            Modifier.AllyCounterAbilities.Add(Lotus);
            Modifier.AllyCounterAbilities.Add(AphoticShield);
            Modifier.AllyCounterAbilities.Add(FortunesEnd);
            Modifier.AllyCounterAbilities.Add(Manta);
            Modifier.AllyCounterAbilities.Add(Eul);
        }

        public EvadableModifier Modifier { get; }

        protected override float GetCastRange()
        {
            return base.GetCastRange() + GetRadius();
        }
    }
}